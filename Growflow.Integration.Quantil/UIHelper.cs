using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Growflo.Integration.Windows
{
    public class UIHelper
    {
        public static void ShowErrorMessage(string text)
        {
            MessageBox.Show(text, "Growflo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        public static void ShowInformationMessage(string text)
        {
            MessageBox.Show(text, "Growflo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
