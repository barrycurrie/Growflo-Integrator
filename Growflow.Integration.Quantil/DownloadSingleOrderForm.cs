using Growflo.Integration.Core.Controllers;
using Growflo.Integration.Core.Entities;
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
    public partial class DownloadSingleOrderForm : Form
    {
        private readonly ISageController _sageController;
        private readonly IWebController _webController;

        public DownloadSingleOrderForm(ISageController sageController, IWebController webController)
        {
            InitializeComponent();
            _sageController = sageController;
            _webController = webController;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                var onlineOrder = _webController.DownloadOrder(orderNumberTextBox.Text);

                if (onlineOrder == null)
                {
                    UIHelper.ShowInformationMessage("Cannot locate specified order");
                    return;
                }

                var mappingHelper = new MappingHelper();
                var sageBatchInvoice = mappingHelper.CreateSageBatchInvoice(onlineOrder);

                var result = _sageController.CreateBatchInvoice(sageBatchInvoice);

                using (var resultsForm = new ResultsForm("Download Orders"))
                {
                    var resultsViewModel = new ResultViewModel()
                    {
                        Action = result.Action,
                        Message = result.Message,
                        Id = result.Id,
                        Result = result.Result.ToString(),
                    };

                    resultsForm.SetData((new ResultViewModel[] { resultsViewModel }));

                    resultsForm.ShowDialog();

                    Close();
                }
            }
            catch(Exception ex)
            {
                UIHelper.ShowErrorMessage(ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
