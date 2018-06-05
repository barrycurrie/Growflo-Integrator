using Growflo.Integration.Core;
using Growflo.Integration.Core.Sage;
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
    public partial class LoginForm : Form
    {
        private ISageController _sageHelper;

        public LoginForm(ISageController sageHelper)
        {
            InitializeComponent();

            _sageHelper = sageHelper;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            usernameTextbox.Text = AppSettings.GetInstance().SageUsername;
            passwordTextBox.Text = AppSettings.GetInstance().SagePassword;

            MinimumSize = MaximumSize = Size;
        }
        private void okButton_Click(object sender, EventArgs e)
        {
            string errorMessage = "";

            try
            { 
                if (_sageHelper.Connect(usernameTextbox.Text, passwordTextBox.Text, ref errorMessage))
                {
                    DialogResult = DialogResult.OK;
                }
                else
                {
                    UIHelper.ShowInformationMessage(errorMessage);
                }
            }
            catch(SageConnectionFailedException)
            {
                UIHelper.ShowErrorMessage("Invalid username and/or password.");
            }
            catch(Exception ex)
            {
                UIHelper.ShowErrorMessage($"An error occurred trying to log into sage:\n\r{ex.Message}");
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}
