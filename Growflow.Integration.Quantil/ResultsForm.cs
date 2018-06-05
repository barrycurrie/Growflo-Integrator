using Growflo.Integration.Core.Sage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Growflo.Integration.Windows
{
    public partial class ResultsForm : Form
    {
        private string _title;

        public ResultsForm(string title)
        {
            InitializeComponent();

            _title = title;
        }

        private void ResultsForm_Load(object sender, EventArgs e)
        {
            txtTitle.Text = _title;
        }

        public void SetData(IEnumerable<ResultViewModel> results)
        {
            try
            {
                resultsListView.BeginUpdate();

                var listViewItems = new List<ListViewItem>();

                foreach (var result in results)
                {
                    var listViewItem = new ListViewItem(new string[] {result.Action, result.Id, result.Result.ToString(), result.Message });
                    listViewItem.Tag = result;
                    listViewItems.Add(listViewItem);
                }

                resultsListView.Items.AddRange(listViewItems.ToArray());
            }
            finally
            {
                resultsListView.EndUpdate();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
