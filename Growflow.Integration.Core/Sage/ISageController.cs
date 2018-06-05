using Growflo.Integration.Core.Sage.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace Growflo.Integration.Core.Sage
{
    public interface ISageController : IDisposable 
    {
        bool IsConnected { get; }
        void VerifyConnection();
        void Disconnect();
        string SageDataPath { get; }
        object Add(object sageObject);
        bool Connect(ref string errorMessage);
        bool Connect(string username, string password, ref string errorMessage);
        object Read(object sageObject, string fieldName);
        void Write(object sageObject, string fieldName, object value);

        SageActionResult<SageCustomer> CreateCustomer(SageCustomer customer);
        SageActionResult<SageBatchInvoice> CreateBatchInvoice(SageBatchInvoice sageInvoice);
        IList<SageCustomer> GetCustomers();

        DataSet GetCustomersData(string[] columns);
        DataSet GetCustomerAddressData(string[] columns);
        DataSet GetStockData(string[] columns, bool includeStockCategories);
        IList<SageNominalCode> GetNominalCodes();
        IList<SageVatRate> GetTaxCodes();
        DataSet GetPriceBandsData(string[] columns);
        DataSet GetInvoicesCreditsData(DateTime? from, DateTime? to, string[] columns);
        DataSet GetNominalCodesData(string[] columns);
        DataSet GetTaxCodesData();
        DataSet GetCurrenciesData();
        //void CreateSalesOrder(DataSet dataSet);
        //bool PostSalesOrder(DataRow salesOrders);
        //bool PostSalesOrders(DataTable salesOrders);
    }
}