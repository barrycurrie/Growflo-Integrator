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
        public DateTime OrderDate { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();
        public string Notes1 { get; internal set; }
        public string Notes2 { get; internal set; }
        public string Notes3 { get; internal set; }
        public string TakenBy { get; internal set; }

        public class Item
        {
            public string StockCode { get; set; }
            public string Description { get; set; }
            public string NominalCode { get; set; }
            public short TaxCode { get; set; }
            public double Quantity { get; set; }
            public double UnitPrice { get; set; }
            public double NetAmount { get; set; }
            public double FullNetAmount { get; set; }
            public string Comment1 { get; set; }
            public string Comment2 { get; set; }
            public string UnitOfSale { get; set; }
            public double TaxRate { get; set; }
        }
    }
}
