using Newtonsoft.Json;
using System.Collections.Generic;
using static Growflo.Integration.Core.Controllers.WebController;

namespace Growflo.Integration.Core.Entities.Web
{
    public class OnlineInvoice
    {
        [JsonProperty("id")]
        public int OrderID { get; set; }

        [JsonProperty("account_name")]
        public string Name { get; set; }

        [JsonProperty("account_identifier")]
        public string AccountIdentifier { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        [JsonProperty("purchase_number")]
        public string PurchaseOrderNo { get; set; }

        [JsonProperty("dates")]
        public OnlineOrderDates Dates { get; set; }

        public bool IsCredit { get; set; }

        [JsonProperty("invoice_number")]
        public string InvoiceNumber { get; set; }

        public RootList<OnlineOrderItem> Items { get; set; }
    }
}
