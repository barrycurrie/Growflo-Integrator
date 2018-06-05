using Growflo.Integration.Core.Entities.Web;
using Growflo.Integration.Core.Sage.Entities;
using System.Collections.Generic;

namespace Growflo.Integration.Core.Controllers
{
    public interface IWebController
    {
        IList<OnlineCustomer> DownloadCustomers();
        IList<OnlineInvoice> DownloadInvoices();
        IList<OnlineCredit> DownloadCredits();
        OnlineInvoice DownloadOrder(string orderNumber);
        void ConfirmOrders(int[] orders);
        void UploadCustomerDetails(IEnumerable<SageCustomer> customers);
        void UploadNominalCodes(IEnumerable<SageNominalCode> nominalCodes);
        void UploadTaxCodes(IEnumerable<SageVatRate> vatRates);
        OnlineCredit DownloadCredit(string creditId);
        void ConfirmCredits(int[] v);
    }
}