using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core
{
    public class AppSettings
    {
        private static AppSettings _instance;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        private AppSettings()
        {
        }

        public static AppSettings GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AppSettings();
                _instance.Initialize();
            }

            return _instance;
        }
        public string SageDsn { get; private set; }
        public string SageDataPath { get; private set; }
        public string SageUsername { get; private set; }
        public string SagePassword { get; private set; }
        public bool EnableSilentLogOn { get; set; }
        public string BaseUri { get; private set; }
        public string AuthorizationKey { get; private set; }
        public string ApplicationDataPath { get; private set; }
        public bool DownloadSingleOrders { get; private set; }
        public string DatabasePath { get; set; }
        public bool PreventDuplicates { get; set; }
        public bool LogJson { get; set; }
        public bool SendEmailNotification { get; set; }
        public string EmailTo { get; set; }
        public string EmailFrom { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpServer { get; set; }
        public string SmtpPassword { get; set; }
        public int SmtpPort { get; set; }
        public bool ConfirmOrders { get; set; }

        private void Initialize()
        {
            _logger.Debug("Initializing Application Settings");

            SageDsn = GetAppSetting(nameof(SageDsn));
            SageDataPath = GetAppSetting(nameof(SageDataPath));
            SageUsername = GetAppSetting(nameof(SageUsername));
            SagePassword = GetAppSetting(nameof(SagePassword));
            BaseUri = GetAppSetting( nameof(BaseUri));
            AuthorizationKey = GetAppSetting(nameof(AuthorizationKey));
            EnableSilentLogOn = GetBoolAppSetting(nameof(EnableSilentLogOn));
            ApplicationDataPath = GetAppSetting(nameof(ApplicationDataPath));
            LogJson = GetBoolAppSetting(nameof(LogJson));
            DownloadSingleOrders = GetBoolAppSetting(nameof(DownloadSingleOrders), false);
            PreventDuplicates = GetBoolAppSetting(nameof(PreventDuplicates), true);
            DatabasePath = GetAppSetting(nameof(DatabasePath), true);

            SendEmailNotification = GetBoolAppSetting(nameof(SendEmailNotification));
            EmailTo = GetAppSetting(nameof(EmailTo), true);
            EmailFrom = GetAppSetting(nameof(EmailFrom), true);
            SmtpServer = GetAppSetting(nameof(SmtpServer), true);
            SmtpUsername = GetAppSetting(nameof(SmtpUsername), true);
            SmtpPassword = GetAppSetting(nameof(SmtpPassword), true);
            SmtpPort = int.Parse(GetAppSetting(nameof(SmtpPort), true));
            ConfirmOrders = GetBoolAppSetting(nameof(ConfirmOrders), false);


            _logger.Debug("Application Settings Initialized");
        }

        private bool GetBoolAppSetting(string appSetting, bool throwException = true)
        {
            appSetting = GetAppSetting(appSetting, throwException);

            return string.Compare(appSetting, "true", true) == 0;
        }

        private string GetAppSetting(string appSetting, bool throwException = true)
        {
            string setting = "";

            try
            {
                setting = ConfigurationManager.AppSettings[appSetting];
            }
            catch (Exception)
            {
                _logger.Debug($"Error reading application setting: {appSetting}.");

                if (throwException)
                    throw;
            }

            _logger.Debug($"Reading Application setting.  Key={appSetting}, Value={setting}");

            return setting;
        }
    }
}
