using Growflo.Integration.Core.Entities;
using Growflo.Integration.Core.Sage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Growflo.Integration.Core.Entities.Web;
using Growflo.Integration.Core.Entities.Sage;

namespace Growflo.Integration.Core.Controllers
{
    public class TaskController
    {

        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IWebController _webController;
        private readonly ISageController _sageController;

        public TaskController(IWebController webController, ISageController sageController)
        {
            _webController = webController ?? throw new ArgumentNullException(nameof(webController));
            _sageController = sageController ?? throw new ArgumentNullException(nameof(sageController));
        }

        public void Sync(bool createNewCustomers)
        {
            StringBuilder sb = new StringBuilder();

            IList<OnlineOrder> newOrders = _webController.GetOrders();

            foreach (OnlineOrder onlineOrder in newOrders)
            {
                SageBatchInvoice sageInvoice = CreateSageInvoice(onlineOrder, false);

                _sageController.CreateBatchInvoice(sageInvoice);
            }

            IList<OnlineOrder> newCredits = _webController.GetCredits();

            foreach (OnlineOrder onlineOrder in newOrders)
            {
                SageBatchInvoice sageInvoice = CreateSageInvoice(onlineOrder, true);

                _sageController.CreateBatchInvoice(sageInvoice);
            }
        }

        private SageBatchInvoice CreateSageInvoice(OnlineOrder onlineOrder, bool isCredit)
        {
            SageBatchInvoice sageBatchInvoice = new SageBatchInvoice()
            {
                InvoiceReference = onlineOrder.Id,
                CustomerAccountNumber = onlineOrder.AccountIdentifier,
                Date = DateTime.Today,
                IsCredit = isCredit,

            };

            return sageBatchInvoice;
        }

        private bool ValidateNominalCodes(IList<SageBatchInvoice> newOrders, ref StringBuilder sb, ref List<string> invalidNominalCodes)
        {
            bool result = true;
            var sageNominalCodes = _sageController.GetNominalCodes().Select(nc => nc.Code);

            foreach (SageBatchInvoice invoice in newOrders)
            {
                foreach (SageBatchInvoice.Split item in invoice.Splits)
                {
                    if (!sageNominalCodes.Contains(item.NominalCode))
                    {
                        string errorMessage = $"Order No: {invoice.SageInvoiceNo}, A/C: {invoice.CustomerAccountNumber} contains an invalid nominal code";
                        sb.AppendFormat(errorMessage);
                        _logger.Debug(errorMessage);
                        result = false;
                    }
                }
            }

            return result;
        }

        public List<SageCustomer> SyncCustomers()
        {
            List<SageCustomer> newCustomers = new List<SageCustomer>();

            try
            {
                _logger.Debug("Synchronizing customers");

                IList<SageCustomer> customers = _webController.DownloadCustomers();

                if (customers.Count > 0)
                {
                    newCustomers.AddRange(_sageController.CreateCustomers(customers, null));
                }
            }
            catch(Exception ex)
            {
                _logger.Error("An error occurred synchronizing customers between Growflo and sage: {0}", ex.Message);
            }

            return newCustomers;
        }


    }
}
