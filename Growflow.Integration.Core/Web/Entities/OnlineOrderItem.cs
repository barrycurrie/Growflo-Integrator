using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Entities.Web
{
    public class OnlineOrderItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quantity")]
        public decimal Quantity { get; set; }

        [JsonProperty("sub_total")]
        public decimal SubTotal { get; set; }

        [JsonProperty("vat_total")]
        public decimal VatTotal { get; set; }

        [JsonProperty("vat_code")]
        public string VatCode { get; set; }

        [JsonProperty("nominal_code")]
        public string NominalCode { get; set; }
    }
}
