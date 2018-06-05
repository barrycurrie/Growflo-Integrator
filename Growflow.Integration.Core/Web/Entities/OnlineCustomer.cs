using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Growflo.Integration.Core.Controllers.WebController;

namespace Growflo.Integration.Core.Entities.Web
{
    public class OnlineCustomer
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("identifier")]
        public string Identifier { get; set; }

        [JsonProperty("email_Address")]
        public string EmailAddress { get; set; }

        [JsonProperty("vat_number")]
        public string VatNumber { get; set; }

        //[JsonProperty("")]
        //public string VatCode { get; set; }

        public RootList<OnlineCustomerAddress> InvoiceAddresses { get; set; }
    }
}
