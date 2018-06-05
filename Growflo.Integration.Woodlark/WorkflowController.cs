using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Data;
using Growflo.Integration.Core.Sage;

namespace Growflo.Integration.Woodlark
{
    public class WorkflowController
    {
        private string _baseFolder;
        private readonly string _downloadedFolder;
        private readonly string _archiveFolder;
        private readonly string _errorFolder;
        private ISageController _sageController;
        private DBController _dbController;

        public WorkflowController(ISageController sageController)
        {
            if (sageController == null)
                throw new ArgumentNullException(nameof(sageController));

            _dbController = new DBController();

            _baseFolder = ConfigurationManager.AppSettings["BaseFolder"];
            _downloadedFolder = Path.Combine(_baseFolder, "Downloaded");
            _archiveFolder = Path.Combine(_baseFolder, "Archive");
            _errorFolder = Path.Combine(_baseFolder, "Errors");
        }

        public void Execute()
        {
            VerifyOrCreateFolders();

            DownloadFiles();

            ImportSalesOrders();
        }

        private void DownloadFiles()
        {

        }

        private void ImportSalesOrders()
        {
            // Get Files.
            foreach (var file in Directory.GetFiles(_downloadedFolder, "SalesOrder*.xml"))
            {
                try
                {
                    System.Data.DataSet dataSet = new System.Data.DataSet();
                    dataSet.ReadXml(file);

                    _sageController.CreateSalesOrder(dataSet);

                }
                catch(Exception ex)
                {

                }
            }
        }

        private void VerifyOrCreateFolders()
        {
            string[] folders = new string[] {_baseFolder, _archiveFolder, _downloadedFolder, _errorFolder };

            foreach (string folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }

        private void SaveSalesOrderInDatabase(DataSet salesOrder)
        {

        }
    }
}
