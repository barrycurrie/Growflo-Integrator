using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Growflo.Integration.Core.Sage.Entities;

namespace Growflo.Integration.Core.Sage
{
    public abstract class BaseSageController : ISageController
    {
        public bool IsConnected { get; protected set; }
        public string SageDataPath { get; protected set; }

        public BaseSageController(string sageDataPath)
        {
            if (!Directory.Exists(sageDataPath))
                throw new DirectoryNotFoundException($"{sageDataPath} does not exist.");

            SageDataPath = sageDataPath;
        }

        public BaseSageController()
            : this(AppSettings.GetInstance().SageDataPath)
        {

        }

        public bool Connect(ref string errorMessage)
        {
            return Connect(AppSettings.GetInstance().SageUsername, AppSettings.GetInstance().SagePassword, ref errorMessage);
        }
        public abstract bool Connect(string username, string password, ref string errorMessage);
        public abstract void VerifyConnection();
        public abstract void Disconnect();
        public abstract void Dispose();


        public abstract object Read(object sageObject, string fieldName);
        public abstract void Write(object sageObject, string fieldName, object value);
        public abstract object Add(object sageObject);
        public abstract SageActionResult<SageBatchInvoice> CreateBatchInvoice(SageBatchInvoice sageInvoice);
        public abstract SageActionResult<SageCustomer> CreateCustomer(SageCustomer customer);
        public abstract IList<SageCustomer> GetCustomers();
        public abstract IList<SageNominalCode> GetNominalCodes();
        public abstract DataSet GetCustomerAddressData(string[] columns);
        public abstract DataSet GetCurrenciesData();
        public abstract DataSet GetCustomersData(string[] columns);
        public abstract DataSet GetInvoicesCreditsData(DateTime? from, DateTime? to, string[] columns);
        public abstract DataSet GetNominalCodesData(string[] columns);
        public abstract DataSet GetPriceBandsData(string[] columns);
        public abstract DataSet GetStockData(string[] columns, bool includeStockCategories);
        public abstract IList<SageVatRate> GetTaxCodes();
        public abstract DataSet GetTaxCodesData();

    }
}
