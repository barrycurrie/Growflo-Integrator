using Growflo.Integration.Core;
using Growflo.Integration.Core.Controllers;
using Growflo.Integration.Core.Email;
using Growflo.Integration.Core.Sage;
using Growflo.Integration.Quantil;
using Growflo.Integration.Quantil.Database;
using Growflo.Integration.Quantil.Workflow;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Growflo.Integration.Windows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string sageDataPath = AppSettings.GetInstance().SageDataPath;
            bool enableSilentLogOn = AppSettings.GetInstance().EnableSilentLogOn;
            bool connected = false;

            using (ISageController sageController = new Sage24Controller(sageDataPath))
            {

                if (enableSilentLogOn)
                {
                    string errorMessage = "";
                    sageController.Connect(AppSettings.GetInstance().SageUsername, AppSettings.GetInstance().SagePassword, ref errorMessage);

                    if (!sageController.IsConnected)
                    {
                        UIHelper.ShowInformationMessage($"Could not log on silently: {errorMessage}");
                    }
                }
                else
                {
                    using (LoginForm logInForm = new LoginForm(sageController))
                    {
                        connected = logInForm.ShowDialog() == DialogResult.OK;
                    }
                }

                if (connected)
                {
                    Application.Run(new MainForm(sageController, new WebController()));
                }
            }
        }
    }
}
