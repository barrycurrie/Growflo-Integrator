using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Entities.Web
{
    public class OnlineVatRate
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
