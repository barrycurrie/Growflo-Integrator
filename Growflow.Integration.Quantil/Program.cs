using Growflo.Integration.Core;
using Growflo.Integration.Core.Controllers;
using Growflo.Integration.Core.Email;
using Growflo.Integration.Core.Sage;
using Growflo.Integration.Quantil;
using Growflo.Integration.Quantil.Database;
using NLog;
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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var config = new NLog.Config.LoggingConfiguration();

            // Targets where to log to: File and Console
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "c:\\programdata\\growflo\\quantil\\log.txt" };

            // Rules for mapping loggers to targets            
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            //Apply config
            NLog.LogManager.Configuration = config;

            _logger.Debug("Application starting");

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
