using Growflo.Integration.Core.Controllers;
using Growflo.Integration.Core.Entities.Web;
using Growflo.Integration.Core.Sage;
using Growflo.Integration.Core.Sage.Entities;
using Growflo.Integration.Quantil.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Quantil.Workflow
{
    public class WorkflowController
    {
        private IWebController _webController;
        private ISageController _sageController;
        private DatabaseController _databaseController;

        public WorkflowController(IWebController webController, DatabaseController databaseController, ISageController sageController)
        {
            if (webController == null)
                throw new ArgumentNullException(nameof(webController));

            if (databaseController == null)
                throw new ArgumentNullException(nameof(databaseController));

            if (sageController == null)
                throw new ArgumentNullException(nameof(sageController));


            _webController = webController;
            _databaseController = databaseController;
            _sageController = sageController;
        }

        private DataTable CreateResultsDataTable()
        {
            var results = new DataTable();
            results.Columns.Add("InvoiceOrCreditNo", typeof(int));
            results.Columns.Add("AccountNo", typeof(string));
            results.Columns.Add("OrderNo", typeof(string));
            results.Columns.Add("Net", typeof(decimal));
            results.Columns.Add("Vat", typeof(decimal));
            results.Columns.Add("Gross", typeof(decimal));
            results.Columns.Add("Result", typeof(bool));
            results.Columns.Add("Message", typeof(string));
            return results;
        }

        public void Run()
        {
            var results = CreateResultsDataTable();

            DownloadAndImportInvoices(results);

            DownloadAndImportCredits(results);
        }

        private int DownloadAndImportInvoices(DataTable results)
        {
            int count = 0;

            var invoices = _webController.DownloadInvoices();

            while (invoices.Count > 0)
            {
                foreach (var invoice in invoices)
                {
                    var dataRow = results.NewRow();
                    dataRow["InvoiceOrCreditNo"] = invoice.InvoiceNumber;
                    dataRow["AccountNo"] = invoice.AccountIdentifier;
                    dataRow["OrderNo"] = invoice.OrderID;
                    dataRow["Result"] = false;
                    dataRow["Message"] = "";

                    try
                    {
                        bool isDuplicate = _databaseController.CheckForInvoiceOrCredit(invoice.InvoiceNumber);

                        if (isDuplicate)
                        {
                            dataRow["Message"] = " Duplicate invoice. ";
                        }
                        else
                        {
                            _databaseController.SaveInvoiceCredit(invoice);

                            var sageInvoice = CreateSageBatchInvoice(invoice);
                            _sageController.CreateBatchInvoice(sageInvoice);

                            _databaseController.SetInvoiceCreditAsImported(invoice.InvoiceNumber);
                        }

                        _webController.ConfirmOrders(new[] { invoice.OrderID });
                        dataRow["Result"] = true;
                        dataRow["Message"] = "OK";
                        count++;
                    }
                    catch (Exception ex)
                    {
                        dataRow["Message"] = ex.Message;
                    }

                    results.Rows.Add(dataRow);
                }

                invoices = _webController.DownloadInvoices();
            }

            return count;
        }


        private int DownloadAndImportCredits(DataTable results)
        {
            int count = 0;

            var credits = _webController.DownloadCredits();

            while (credits.Count > 0)
            {
                foreach (var credit in credits)
                {
                    var dataRow = results.NewRow();
                    dataRow["InvoiceOrCreditNo"] = credit.CreditNumber;
                    dataRow["AccountNo"] = credit.AccountIdentifier;
                    dataRow["OrderNo"] = "";
                    dataRow["Result"] = false;
                    dataRow["Message"] = "";

                    try
                    {
                        bool isDuplicate = _databaseController.CheckForInvoiceOrCredit(credit.CreditNumber);

                        if (isDuplicate)
                        {
                            dataRow["Message"] = " Duplicate invoice. ";
                        }
                        else
                        {
                            _databaseController.SaveInvoiceCredit(credit);

                            var sageInvoice = CreateSageBatchInvoice(credit);
                            _sageController.CreateBatchInvoice(sageInvoice);

                            _databaseController.SetInvoiceCreditAsImported(credit.InvoiceNumber);
                        }

                        int creditNumber = int.Parse(credit.CreditNumber);
                        _webController.ConfirmCredits(new int[] { creditNumber});

                        dataRow["Result"] = true;
                        dataRow["Message"] = "OK";
                        count++;
                    }
                    catch (Exception ex)
                    {
                        dataRow["Message"] = ex.Message;
                    }

                    results.Rows.Add(dataRow);
                }

                credits = _webController.DownloadCredits();
            }

            return count;
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


        public SageCustomer CreateSageCustomer(OnlineCustomer onlineCustomer)
        {
            var sageCustomer = new SageCustomer()
            {
                Name = onlineCustomer.Name,
                AccountNumber = onlineCustomer.Identifier,
                Email = onlineCustomer.EmailAddress,
                VatNumber = onlineCustomer.VatNumber
            };

            var invoiceAddress = onlineCustomer.InvoiceAddresses.data.FirstOrDefault();

            if (invoiceAddress != null)
            {
                sageCustomer.InvoiceAddressLine1 = invoiceAddress.Line1;
                sageCustomer.InvoiceAddressLine2 = invoiceAddress.Line2;
                sageCustomer.InvoiceAddressLine3 = invoiceAddress.Town;
                sageCustomer.InvoiceAddressLine4 = invoiceAddress.County;
                sageCustomer.InvoiceAddressLine5 = invoiceAddress.Postcode;
            }

            return sageCustomer;
        }
    }
}
