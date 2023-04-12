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
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
        }

        public DateTime? SelectedDate { get; set; }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                SelectedDate = fromDateTimePicker.Value;
                DialogResult = DialogResult.OK;
            }
            catch(Exception ex)
            {
                UIHelper.ShowErrorMessage(ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
