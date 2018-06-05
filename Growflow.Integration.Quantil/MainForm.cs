using Growflo.Integration.Core;
using Growflo.Integration.Core.Controllers;
using Growflo.Integration.Core.Entities;
using Growflo.Integration.Core.IO;
using Growflo.Integration.Core.Sage;
using Growflo.Integration.Core.Sage.Entities;
using Growflo.Integration.Quantil;
using Growflo.Integration.Quantil.Database;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Growflo.Integration.Windows
{
    public partial class MainForm : Form
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private ISageController _sageController;
        private IWebController _webController;
        private MappingHelper _mappingHelper;

        public object Results { get; private set; }

        public MainForm(ISageController sageController, IWebController webController)
        {
            InitializeComponent();

            _sageController = sageController ?? throw new ArgumentNullException(nameof(sageController));
            _webController = webController ?? throw new ArgumentNullException(nameof(webController));
            _mappingHelper = new MappingHelper();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            downloadSingleOrderButton.Visible = AppSettings.GetInstance().DownloadSingleOrders;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void uploadCustomerDetailsButtonClick(object sender, EventArgs e)
        {
            using (WaitCursor waitCursor = new WaitCursor(this))
            {
                try
                {
                    var customers = _sageController.GetCustomers();

                    if (customers == null || customers.Count == 0)
                    {
                        UIHelper.ShowInformationMessage("No customers found to upload");
                        return;
                    }

                    _webController.UploadCustomerDetails(customers);

                    UIHelper.ShowInformationMessage($"Updated customer details for {customers.Count} accounts.");
                }
                catch(Exception ex)
                {
                    UIHelper.ShowErrorMessage(ex.Message);
                }
            }
        }

        private void uploadNominalCodesButton_Click(object sender, EventArgs e)
        { 
            using (WaitCursor waitCursor = new WaitCursor(this))
            {
                try
                {
                    var nominalCodes = _sageController.GetNominalCodes();

                    if (nominalCodes == null || nominalCodes.Count == 0)
                    {
                        UIHelper.ShowInformationMessage("No nominal codes found to upload");
                        return;
                    }
                    _webController.UploadNominalCodes(nominalCodes);

                    UIHelper.ShowInformationMessage($"Updated {nominalCodes.Count} nominal codes.");
                }
                catch (Exception ex)
                {
                    UIHelper.ShowErrorMessage(ex.Message);
                }
            }
        }

        private void uploadTaxCodesButton_Click(object sender, EventArgs e)
        {
            using (WaitCursor waitCursor = new WaitCursor(this))
            {
                try
                {
                    var sageTaxCodes = _sageController.GetTaxCodes().Where(t => !string.IsNullOrEmpty(t.Description)).ToList();

                    if (sageTaxCodes == null || sageTaxCodes.Count == 0)
                    {
                        UIHelper.ShowInformationMessage("No tax codes codes found to upload");
                        return;
                    }

                    _webController.UploadTaxCodes(sageTaxCodes);

                    UIHelper.ShowInformationMessage($"Updated {sageTaxCodes.Count} tax codes.");

                }
                catch (Exception ex)
                {
                    UIHelper.ShowErrorMessage(ex.Message);
                }
            }
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            try
            {
                var results = new List<ISageActionResult>();
                results.AddRange(DownloadCustomers().Where(r => r.Result == SageActionResultType.Success).Cast<ISageActionResult>().ToList());

                DownloadInvoicesAndCredits();
            }
            catch (Exception ex)
            {
                UIHelper.ShowErrorMessage(ex.Message);
            }
        }

        private void DownloadInvoicesAndCredits()
        {
            using (var progressForm = new DownloadOrdersAndCreditsForm(_webController, new DatabaseController(), _sageController))
            {
                progressForm.ShowDialog();
            }
        }

        private List<SageActionResult<SageCustomer>> DownloadCustomers()
        {
            var results = new List<SageActionResult<SageCustomer>>();

            var onlineCustomers = _webController.DownloadCustomers().ToList();

            if (onlineCustomers == null || onlineCustomers.Count == 0)
            {
                UIHelper.ShowInformationMessage("There were no customers found to download.");
                return results;
            }

            using (var form = new DownloadCustomersForm(_sageController, onlineCustomers))
            {
                form.ShowDialog();
                results.AddRange(form.Results);
            }

            return results;
        }

        private void downloadSingleOrderButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var form = new DownloadSingleOrderForm(_sageController, _webController))
                {
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowErrorMessage(ex.Message);
            }
        }

        private void viewSettingsButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (SettingsForm settingsForm = new SettingsForm())
                {
                    settingsForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowErrorMessage(ex.Message);
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
