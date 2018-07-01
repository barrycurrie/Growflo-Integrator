using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Data;
using Growflo.Integration.Core.Sage;
using Growflo.Integration.Core.Sage.Entities;
using Growflo.Integration.Woodlark.Database;
using Growflo.Integration.Core.Extensions;

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

            //_dbController = new DBController();

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

                    DataRow order = dataSet.Tables["Order"].AsEnumerable().FirstOrDefault();
                    DataRow[] orderItems = dataSet.Tables["Order_Item"].AsEnumerable().ToArray();

                    SageSalesOrderPost sageSalesOrderPost = CreateSageSalesOrderPost(order, orderItems);

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

        private SageSalesOrderPost CreateSageSalesOrderPost(DataRow salesOrderDataRow, DataRow[] salesOrderLinesDataRows)
        {
            SageSalesOrderPost sageSalesOrderPost = new SageSalesOrderPost();
            sageSalesOrderPost.CustomerAccountNumber = salesOrderDataRow.GetString("ACCOUNT_NUMBER");
            //sageSalesOrderPost.Analysis1 = salesOrderDataRow.GetString("ORDER_ID");
            //sageSalesOrderPost.Analysis2 = salesOrderDataRow.GetString("ORDER_REFERENCE");
            sageSalesOrderPost.OrderDate = salesOrderDataRow.GetDateTime("ORDER_DATE");
            sageSalesOrderPost.DeliveryAddress1 = salesOrderDataRow.GetString("DELIVERY_ADDRESS1");
            sageSalesOrderPost.DeliveryAddress1 = salesOrderDataRow.GetString("DELIVERY_ADDRESS2");
            sageSalesOrderPost.DeliveryAddress1 = salesOrderDataRow.GetString("DELIVERY_ADDRESS3");
            sageSalesOrderPost.DeliveryAddress1 = salesOrderDataRow.GetString("DELIVERY_ADDRESS4");
            sageSalesOrderPost.DeliveryAddress1 = salesOrderDataRow.GetString("DELIVERY_POSTCODE");

            foreach (DataRow item in salesOrderLinesDataRows)
            {
                SageSalesOrderPost.Item postItem = new SageSalesOrderPost.Item();

            }


            return sageSalesOrderPost;
        }
    }
}
