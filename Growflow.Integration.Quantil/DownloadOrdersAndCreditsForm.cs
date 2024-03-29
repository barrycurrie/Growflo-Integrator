﻿using Growflo.Integration.Core;
using Growflo.Integration.Core.Controllers;
using Growflo.Integration.Core.Email;
using Growflo.Integration.Core.Entities.Web;
using Growflo.Integration.Core.Sage;
using Growflo.Integration.Core.Sage.Entities;
using Growflo.Integration.Quantil.Database;
using Growflo.Integration.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Growflo.Integration.Quantil
{
    public partial class DownloadOrdersAndCreditsForm : Form
    {
        private IWebController _webController;
        private IDatabaseController _databaseController;
        private ISageController _sageController;
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public DataSet Results { get; private set; }

        public DownloadOrdersAndCreditsForm(IWebController webController, IDatabaseController databaseController, ISageController sageController)
        {
            InitializeComponent();
            progressBar.MarqueeAnimationSpeed = 20;

            if (webController == null)
                throw new ArgumentNullException(nameof(webController));

            if (databaseController == null)
                throw new ArgumentNullException(nameof(databaseController));

            if (sageController == null)
                throw new ArgumentNullException(nameof(sageController));


            _webController = webController;
            _databaseController = databaseController;
            _sageController = sageController;

            CreateResultsDataTable();
        }

        private void ProgressForm_Shown(object sender, EventArgs e)
        {
            try
            {
                string errorMessage = "";

                if (!_databaseController.CheckConnection(ref errorMessage))
                {
                    throw new Exception(errorMessage);
                }

                CreateResultsDataTable();

                DownloadAndImportInvoices();

                DownloadAndImportCredits();

                string basePath = Growflo.Integration.Core.AppSettings.GetInstance().ApplicationDataPath;
                string fileName = System.IO.Path.Combine(basePath, "import_" + DateTime.Now.ToString("yy-MM-dd HH-mm-ss"));

                Results.WriteXml(fileName + ".xml");

                var fileController = new Growflo.Integration.Core.IO.FileController();
                string csv = fileName + ".csv";
                fileController.WriteToCsv(Results.Tables[0], csv);

                SendEmail(csv);

                System.Diagnostics.Process.Start(fileName + ".csv");

                UIHelper.ShowInformationMessage("Import complete!");
            }
            catch (Exception ex)
            {
                UIHelper.ShowErrorMessage("An unexpected error occurred:" + ex.Message);
            }

            Close();
        }

        private void SendEmail(string csv)
        {
            var emailController = new EmailController(AppSettings.GetInstance().SmtpServer,
                AppSettings.GetInstance().SmtpUsername, AppSettings.GetInstance().SmtpPassword,
                AppSettings.GetInstance().SmtpPort);

            emailController.Send(AppSettings.GetInstance().EmailTo, AppSettings.GetInstance().EmailFrom, "Quantil Import Results", csv, csv);
        }

        private void CreateResultsDataTable()
        {
            var results = new DataTable("InvoicesAndCredits");
            results.Columns.Add("InvoiceOrCreditNo", typeof(int));
            results.Columns.Add("AccountNo", typeof(string));
            results.Columns.Add("OrderNo", typeof(string));
            results.Columns.Add("Result", typeof(bool));
            results.Columns.Add("Message", typeof(string));

            Results = new DataSet("Results");
            Results.Tables.Add(results);
        }

        private void DownloadAndImportInvoices()
        {
            _logger.Debug("Download and import invoices: begin");

            int downloaded = 0;
            int success = 0;
            int errors = 0;

            var invoices = _webController.DownloadInvoices();
            _logger.Debug($"{invoices.Count} downloaded.");


            while (invoices.Count > 0)
            {
                downloaded += invoices.Count;

                foreach (var invoice in invoices)
                {
                    _logger.Debug($"Processing invoice: {invoice.InvoiceNumber}: Begin");

                    UpdateInvoiceLabels(downloaded, success, errors);

                    var dataRow = Results.Tables[0].NewRow();
                    dataRow["InvoiceOrCreditNo"] = invoice.InvoiceNumber;
                    dataRow["AccountNo"] = invoice.AccountIdentifier;
                    dataRow["OrderNo"] = invoice.OrderID;
                    dataRow["Result"] = false;
                    dataRow["Message"] = "";

                    try
                    {
                        bool isDuplicate = _databaseController.CheckForInvoice(invoice);

                        if (isDuplicate)
                        {
                            _webController.ConfirmOrders(new[] { invoice.OrderID });
                            errors++;
                            throw new Exception(" Duplicate invoice. ");
                        }

                        var sageInvoice = CreateSageBatchInvoice(invoice);

                        ValidateSageInvoice(sageInvoice);

                        _sageController.CreateBatchInvoice(sageInvoice);
                        _logger.Debug($"Processing {invoice.InvoiceNumber}: Imported into Sage");

                        _databaseController.SaveInvoiceCredit(invoice);
                        _databaseController.SetInvoiceCreditAsImported(invoice.InvoiceNumber);
                        _logger.Debug($"Processing {invoice.InvoiceNumber}: Growflo updated.");

                        _webController.ConfirmOrders(new[] { invoice.OrderID });
                        dataRow["Result"] = true;
                        dataRow["Message"] = "OK";
                        success++;

                        _logger.Debug($"Processing invoice: {invoice.InvoiceNumber}: Success");
                    }
                    catch (Exception ex)
                    {
                        errors++;
                        dataRow["Message"] = ex.Message;
                        _logger.Debug($"Processing {invoice.InvoiceNumber} failed: {ex.Message}");

                    }

                    Results.Tables[0].Rows.Add(dataRow);
                }

                invoices = _webController.DownloadInvoices();
                _logger.Debug($"{invoices.Count} downloaded.");
            }

            _logger.Debug("Downloadload and import invoices: complete");
        }

        private void ValidateSageInvoice(SageBatchInvoice sageInvoice)
        {
            if (sageInvoice.Splits.Any(si => string.IsNullOrWhiteSpace(si.NominalCode))) throw new Exception("One or more items have a blank nominal code");
        }

        private void UpdateInvoiceLabels(int downloaded, int success, int errors)
        {
            downloadedInvoicesCountLabel.Text = downloaded.ToString();
            importedInvoicesCountLabel.Text = success.ToString();
            errorInvoicesCountLabel.Text = errors.ToString();
            Application.DoEvents();
        }

        private void UpdateCreditLabels(int downloaded, int success, int errors)
        {
            downloadedCreditsCountLabel.Text = downloaded.ToString();
            importedCreditsCountLabel.Text = success.ToString();
            errorCreditsCountLabel.Text = errors.ToString();
            Application.DoEvents();
        }

        private void DownloadAndImportCredits()
        {
            _logger.Debug("Downloadload and import credits: begin");

            int downloaded = 0;
            int success = 0;
            int errors = 0;

            var credits = _webController.DownloadCredits();
            _logger.Debug($"{credits.Count} downloaded.");


            while (credits.Count > 0)
            {
                downloaded += credits.Count;

                foreach (var credit in credits)
                {
                    _logger.Debug($"Processing credit: {credit.CreditNumber}: Begin");


                    UpdateCreditLabels(downloaded, success, errors);

                    var dataRow = Results.Tables[0].NewRow();
                    dataRow["InvoiceOrCreditNo"] = credit.CreditNumber;
                    dataRow["AccountNo"] = credit.AccountIdentifier;
                    dataRow["OrderNo"] = "";
                    dataRow["Result"] = false;
                    dataRow["Message"] = "";

                    try
                    {
                        bool isDuplicate = _databaseController.CheckForCredit(credit);
                        int creditNumber = int.Parse(credit.CreditNumber);

                        if (isDuplicate)
                        {
                            _webController.ConfirmCredits(new[] { credit.Id });
                            throw new Exception(" Duplicate credit. ");
                        }

                        var sageInvoice = CreateSageBatchInvoice(credit);
                        _sageController.CreateBatchInvoice(sageInvoice);
                        _logger.Debug($"Processing {credit.CreditNumber}: Imported into Sage");

                        _databaseController.SaveInvoiceCredit(credit);
                        _databaseController.SetInvoiceCreditAsImported(credit.CreditNumber);
                        _logger.Debug($"Processing {credit.CreditNumber}: DB Updated");


                        _webController.ConfirmCredits(new int[] { credit.Id });
                        _logger.Debug($"Processing {credit.CreditNumber}: Growflo updated");

                        success++;
                        dataRow["Result"] = true;
                        dataRow["Message"] = "OK";

                        _logger.Debug($"Processing {credit.CreditNumber}: Success");

                    }
                    catch (Exception ex)
                    {
                        errors++;
                        dataRow["Message"] = ex.Message;
                        _logger.Debug($"Processing {credit.CreditNumber} failed: {ex.Message}");
                    }

                    Results.Tables[0].Rows.Add(dataRow);
                }

                credits = _webController.DownloadCredits();
                _logger.Debug($"{credits.Count} downloaded.");
            }

            _logger.Debug("Downloadload and import credits: complete");
        }



        public SageBatchInvoice CreateSageBatchInvoice(OnlineInvoice onlineOrder)
        {
            SageBatchInvoice sageBatchInvoice = new SageBatchInvoice()
            {
                InvoiceReference = onlineOrder.InvoiceNumber,
                CustomerAccountNumber = onlineOrder.AccountIdentifier,
                Date = onlineOrder.Dates == null ? DateTime.Today : onlineOrder.Dates.DeliveryDate.GetValueOrDefault(DateTime.Today),
                IsCredit = onlineOrder.IsCredit,
                Currency = onlineOrder.CurrencyCode
            };



            var splits = onlineOrder.Items.data.Select(
                x => new SageBatchInvoice.Split()
                {
                    NominalCode = x.NominalCode,
                    VatCode = x.VatCode,
                    VatAmount = (double)x.VatTotal,
                    NetAmount = (double)x.SubTotal,
                    Details = x.Name,
                    Date = sageBatchInvoice.Date
                });

            sageBatchInvoice.Splits.AddRange(splits);

            return sageBatchInvoice;
        }

        public SageBatchInvoice CreateSageBatchInvoice(OnlineCredit onlineOrder)
        {
            SageBatchInvoice sageBatchInvoice = new SageBatchInvoice()
            {
                InvoiceReference = onlineOrder.CreditNumber,
                CustomerAccountNumber = onlineOrder.AccountIdentifier,
                Date = DateTime.Today,
                IsCredit = onlineOrder.IsCredit,
                Currency = onlineOrder.CurrencyCode
            };



            var splits = onlineOrder.Items.data.Select(
                x => new SageBatchInvoice.Split()
                {
                    NominalCode = x.NominalCode,
                    VatCode = x.VatCode,
                    VatAmount = (double)x.VatTotal,
                    NetAmount = (double)x.SubTotal,
                    Details = x.Name,
                    Date = sageBatchInvoice.Date
                });

            sageBatchInvoice.Splits.AddRange(splits);

            return sageBatchInvoice;
        }
    }
}
