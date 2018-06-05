using Growflo.Integration.Core;
using Growflo.Integration.Core.Controllers;
using Growflo.Integration.Core.Entities;
using Growflo.Integration.Core.Entities.Web;
using Growflo.Integration.Core.IO;
using Growflo.Integration.Core.Sage;
using Growflo.Integration.Quantil.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Quantil
{
    public class DownloadController
    {
        private DatabaseController _dbController;
        private IWebController _webController;
        private ISageController _sageController;
        private MappingHelper _mappingHelper;
        private bool _preventDuplicates = true;


        private DataTable createDataSet()
        {
            var results = new DataTable("Results");
            results.Columns.Add("OrderId", typeof(string));
            results.Columns.Add("Downloaded", typeof(bool));
            results.Columns.Add("Result", typeof(bool));
            results.Columns.Add("ErrorMessage", typeof(string));
            return results;
        }

        public DownloadController(IWebController webController, ISageController sageController)
        {
            _webController = webController;
            _sageController = sageController;
            _dbController = new DatabaseController();
            _mappingHelper = new MappingHelper();
        }

        public DataTable Download()
        {
            var results = createDataSet();

            // Import orders to staging table.
            DownloadAndSaveOrders();

            var onlineOrders = _dbController.GetOnlineOrdersToImport();

            foreach (var onlineOrder in onlineOrders)
            {
                string message = "";
                bool imported = false;

                try
                {
                    var sageBatchInvoice = _mappingHelper.CreateSageBatchInvoice(onlineOrder);

                    _sageController.CreateBatchInvoice(sageBatchInvoice);
                    imported = true;

                    _dbController.MarkAsImported(onlineOrder.OrderID);
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                }

                var result = results.NewRow();
                result["OrderId"] = onlineOrder.OrderID;
                result["Result"] = imported;
                result["Message"] = message;
                results.Rows.Add(result);
            }

            return results;
        }

        private void CreateLogs(DataTable results)
        {
            string basePath = Growflo.Integration.Core.AppSettings.GetInstance().ApplicationDataPath;
            string fileName = Path.Combine(basePath, "import_" + DateTime.Now.ToString("yy-MM-dd HH-mm-ss"));

            results.WriteXml(fileName + ".xml");

            var fileController = new FileController();
            fileController.WriteToCsv(results, fileName + ".csv");

            System.Diagnostics.Process.Start(fileName + ".csv");
        }


        private int DownloadAndSaveOrders()
        {
            var downloadedOrders = _webController.DownloadInvoices();
            int total = downloadedOrders.Count;

            while (downloadedOrders.Count > 0)
            {
                foreach (var downloadedOrder in downloadedOrders)
                {

                    if (!_dbController.CheckForInvoice(downloadedOrder.OrderID))
                    {
                        _dbController.SaveInvoiceCredit(downloadedOrder);
                    }

                    //_webController.ConfirmOrders(new int[] { downloadedOrder.Id });
                }

                downloadedOrders = _webController.DownloadInvoices();
                total += downloadedOrders.Count;
            }

            return total;
        }
    }
}
