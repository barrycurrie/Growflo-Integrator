using Growflo.Integration.Core;
using Growflo.Integration.Core.Sage;
using Growflo.Integration.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflow.Integration.IntegrationTests
{
    class Program
    {
        private static string _path;

        static void Main(string[] args)
        {
            _path = AppSettings.GetInstance().ApplicationDataPath; 

            using (ISageController sageController = new Sage24Controller())
            {
                string errorMessage = "";

                if (sageController.Connect(ref errorMessage))
                {
                    //GetStockItemData(sageController);

                    //GetCustomerData(sageController);

                    //GetTaxCodeData(sageController);

                    //GetCustomerAddressData(sageController);

                    //GetNominalCodesData(sageController);

                    //GetPriceBandData(sageController);

                    //ShowStockData(sageController);

                    GetInvoiceData(sageController);
                }
            }
        }

        private static void GetInvoiceData(ISageController sageController)
        {
            Console.WriteLine("Getting invoice data");

            var dataSet = sageController.GetInvoicesCreditsData(DateTime.Today.AddDays(-90), null, null);

            try
            {

                DataSet invoices = dataSet.Copy();
                invoices.DataSetName = "Invoices";
                invoices.Tables["CREDITS"].ChildRelations.Clear();
                invoices.Tables["CREDITS"].Constraints.Clear();
                invoices.Tables["CREDITITEMS"].ParentRelations.Clear();
                invoices.Tables["CREDITITEMS"].Constraints.Clear();
                invoices.Tables.Remove("CREDITS");
                invoices.Tables.Remove("CREDITITEMS");
                invoices.WriteXml(Path.Combine(_path, "Invoices.xml"));


                DataSet credits = dataSet.Copy();
                credits.DataSetName = "Credits";
                credits.Tables["INVOICES"].ChildRelations.Clear();
                credits.Tables["INVOICES"].Constraints.Clear();
                credits.Tables["INVOICEITEMS"].ParentRelations.Clear();
                credits.Tables["INVOICEITEMS"].Constraints.Clear();
                credits.Tables.Remove("INVOICES");
                credits.Tables.Remove("INVOICEITEMS");
                credits.WriteXml(Path.Combine(_path, "Credits.xml"));
            }
            catch(Exception ex)
            {

            }

            string message = "";
        }

        private static void GetPriceBandData(ISageController sageController)
        {
            Console.WriteLine("Getting price band data....");
            string[] columns = new string[] { "ACCOUNT_REF", "NAME" };

            var dataSet = sageController.GetPriceBandsData(columns);
            dataSet.WriteXml(Path.Combine(_path, "PriceBands.xml"));
        }

        private static void GetNominalCodesData(ISageController sageController)
        {
            Console.WriteLine("Getting nominal code data....");
            string[] columns = new string[] { "ACCOUNT_REF", "NAME" };

            var dataSet = sageController.GetNominalCodesData(columns);
            dataSet.WriteXml(Path.Combine(_path, "NominalCodes.xml"));
        }

        private static DataSet GetTaxCodeData(ISageController sageController)
        {
            Console.WriteLine("Getting tax code data....");

            var taxCodesDataSet = sageController.GetTaxCodesData();
            taxCodesDataSet.WriteXml(Path.Combine(_path, "TaxCodes.xml"));

            return taxCodesDataSet;
        }

        private static DataSet GetStockItemData(ISageController sageController)
        {
            Console.WriteLine("Getting stock item data...");

            string[] columns = new string[] { "STOCK_CODE", "DESCRIPTION", "STOCK_CAT", "PRODUCT_BARCODE", "TAX_CODE", "SALES_PRICE" };

            var stockItemsDataSet = sageController.GetStockData(columns, true);
            stockItemsDataSet.WriteXml(Path.Combine(_path, "StockItems.xml"));

            return stockItemsDataSet;
        }

        private static void GetCustomerData(ISageController sageController)
        {
            Console.WriteLine("Getting customer account data...");

            string[] columns = new string[] { "ACCOUNT_REF", "NAME", "BALANCE", "ADDRESS_1", "ADDRESS_2", "ADDRESS_3", "ADDRESS_4", "ADDRESS_5", "COUNTRY_CODE", "PRICE_LIST_REF" };

            var dataSet = sageController.GetCustomersData(columns);
            dataSet.WriteXml(Path.Combine(_path, "customers.xml"));
        }

        private static void GetCustomerAddressData(ISageController sageController)
        {
            Console.WriteLine("Getting customer address data...");
            string[] columns = new string[] { "ADDRESS_TYPE", "ACCOUNT_REF", "NAME", "ADDRESS_1", "ADDRESS_2", "ADDRESS_3", "ADDRESS_4", "ADDRESS_5", "COUNTRY_CODE"};

            var dataSet = sageController.GetCustomerAddressData(columns);

            DataTable deliveryAddresses = dataSet.Tables[0].AsEnumerable()
                                            .Where(row => row.Field<short>("ADDRESS_TYPE") == 0)
                                            .CopyToDataTable();

            deliveryAddresses.TableName = "DeliveryAddresses";

            deliveryAddresses.WriteXml(Path.Combine(_path, "DeliveryAddresses.xml"));
        }

        public static void ShowStockData(ISageController sageController)
        {
            var stockData = GetStockItemData(sageController);

            using (var form = new ReportForm("Stock", stockData))
            {
                form.ShowDialog();
            }
        }
    }
}
