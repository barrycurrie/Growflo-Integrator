using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Growflo.Integration.Core.Controllers.WebController;

namespace Growflo.Integration.Core.Entities.Web
{
    public class OnlineCredit
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("credit_number")]
        public string CreditNumber { get; set; }

        [JsonProperty("invoice_number")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("account_identifier")]
        public string AccountIdentifier { get; set; }

        [JsonProperty("account_name")]
        public string Name { get; set; }

        [JsonProperty("currency_code")]
        public string CurrencyCode { get; set; }

        public bool IsCredit { get; set; } = true;

        public RootList<OnlineOrderItem> Items { get; set; }
    }
}
