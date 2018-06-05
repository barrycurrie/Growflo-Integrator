using Growflo.Integration.Core.Controllers;
using Growflo.Integration.Core.Entities;
using Growflo.Integration.Core.Entities.Web;
using Growflo.Integration.Core.Sage;
using Growflo.Integration.Core.Sage.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Growflo.Integration.Windows
{
    public partial class DownloadCustomersForm : Form
    {
        private List<OnlineCustomer> _onlineCustomers;
        private ISageController _sageController;
        private MappingHelper _mappingHelper;
        private BackgroundWorker _worker;

        public List<SageActionResult<SageCustomer>> Results { get; private set; } = new List<SageActionResult<SageCustomer>>();

        public DownloadCustomersForm(ISageController sageController, List<OnlineCustomer> onlineCustomers)
        {
            InitializeComponent();

            _sageController = sageController;
            _onlineCustomers = onlineCustomers;
            _total = onlineCustomers.Count;
            _mappingHelper = new MappingHelper();
        }

        private void DownloadCustomersForm_Load(object sender, EventArgs e)
        {
            MinimizeBox = false;
            MaximizeBox = false;
            MinimumSize = Size;
            MaximumSize = Size;

        }

        private void DownloadOrdersForm_Shown(object sender, EventArgs e)
        {
            progressBar.Maximum = _onlineCustomers.Count;

            _worker = new BackgroundWorker();
            _worker.DoWork += DownloadAndImportCustomers;
            _worker.WorkerReportsProgress = true;
            _worker.ProgressChanged += ProgressChanged;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.RunWorkerAsync();
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = _current;
            progressLabel.Text = $"Updating {progressBar.Value} of {_onlineCustomers.Count} customers in sage 50.";
            Application.DoEvents();
        }

        private int _current = 0;
        private int _total;

        private void DownloadAndImportCustomers(object sender, DoWorkEventArgs e)
        {
            try
            {
                foreach (var customer in _onlineCustomers)
                {
                    _current++;
                    int progress = ((_current / _onlineCustomers.Count) * 100);
                    _worker.ReportProgress(progress, customer);


                    var sageCustomer = _mappingHelper.CreateSageCustomer(customer);
                    var result = _sageController.CreateCustomer(sageCustomer);
                    Results.Add(result);
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowErrorMessage(ex.Message);
            }
        }
    }
}
