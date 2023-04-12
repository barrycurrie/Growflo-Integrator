using Growflo.Integration.Core;
using Growflo.Integration.Core.Entities.Web;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Quantil.Database
{
    public class SageOdbcDatabaseController : IDatabaseController
    {
        private string _sageDsn;
        private string _sageUsername;
        private string _sagePassword;

        public SageOdbcDatabaseController()
        {
            _sageDsn = AppSettings.GetInstance().SageDsn;
            _sageUsername = AppSettings.GetInstance().SageUsername;
            _sagePassword = AppSettings.GetInstance().SagePassword;
        }

        public bool CheckConnection(ref string errorMessage)
        {
            try
            {
                using (OdbcConnection connection = new OdbcConnection(GetConnectionString()))
                {
                    connection.Open();
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool CheckForCredit(OnlineCredit credit)
        {
            using (OdbcConnection connection = new OdbcConnection(GetConnectionString()))
            {
                using (OdbcCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandText =
                        " select count(*) from audit_header " +
                        $" where Type = 'SC' and Account_Ref = '{credit.AccountIdentifier}' " +
                        $"and Inv_Ref_Numeric = {credit.CreditNumber} and Record_Deleted = 0 ";


                    var result = (int)command.ExecuteScalar();

                    return result > 0;
                }
            }
        }

        public bool CheckForInvoice(OnlineInvoice invoice)
        {
            using (OdbcConnection connection = new OdbcConnection(GetConnectionString()))
            {
                using (OdbcCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    command.CommandText =
                        " select count(*) from audit_header " +
                        $" where Type = 'SI' and Account_Ref = '{invoice.AccountIdentifier}' " +  
                        $"and Inv_Ref_Numeric = {invoice.InvoiceNumber} and Record_Deleted = 0 ";


                    var result = (int)command.ExecuteScalar();

                    return result > 0;
                }
            }
        }


        public void SaveInvoiceCredit(OnlineInvoice invoice)
        {
            
        }

        public void SaveInvoiceCredit(OnlineCredit credit)
        {
            
        }

        public void SetInvoiceCreditAsImported(string invoiceCreditID)
        {
            
        }

        private string GetConnectionString()
        {
            return $"DSN={_sageDsn};Uid={_sageUsername};Pwd={_sagePassword};";
        }
    }
}
