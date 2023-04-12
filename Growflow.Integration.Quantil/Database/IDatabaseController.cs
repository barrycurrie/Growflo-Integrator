using Growflo.Integration.Core.Entities.Web;

namespace Growflo.Integration.Quantil.Database
{
    public interface IDatabaseController
    {
        bool CheckConnection(ref string errorMessage);
        void SaveInvoiceCredit(OnlineInvoice invoice);
        void SaveInvoiceCredit(OnlineCredit credit);
        void SetInvoiceCreditAsImported(string invoiceCreditID);
        bool CheckForInvoice(OnlineInvoice invoice);
        bool CheckForCredit(OnlineCredit credit);

    }
}