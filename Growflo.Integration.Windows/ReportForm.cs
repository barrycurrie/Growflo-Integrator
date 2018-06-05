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
        string _title;
        DataSet _dataSet;

        public ReportForm(string title, DataSet dataSet)
        {
            InitializeComponent();

            _title = title;
            _dataSet = dataSet;
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            dataListView.DataSource = _dataSet.Tables[0];
            dataListView.AutoGenerateColumns = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void printButton_Click(object sender, EventArgs e)
        {
             
        }
    }
}
