using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Sage.Entities
{
    public class SageSalesOrderPost
    {
        public string  CustomerAccountNumber { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string DeliveryAddress3 { get; set; }
        public string DeliveryAddress4 { get; set; }
        public string DeliveryAddress5 { get; set; }
        public string CustomerTelephoneNumber { get; set; }
        public string ContactName { get; set; }
        public string GlobalVatCode { get; set; }

    }
}
