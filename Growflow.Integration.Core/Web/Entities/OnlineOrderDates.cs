using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Entities.Web
{
    public class OnlineOrderDates
    {
        [JsonProperty("ordered_for")]
        public DateTime? OrderedForDate { get; set; }


        [JsonProperty("delivery_Date")]
        public DateTime? DeliveryDate { get; set; }


        [JsonProperty("created_at")]
        public DateTime? CreatedDate { get; set; }
    }
}
