using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Deserializers;
using Newtonsoft.Json;

namespace Growflo.Integration.Core.Sage.Entities
{
    public class SageCustomer : ISageEntity
    {
        public string AccountNumber { get; set; }
        public string  Name { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public string VatNumber { get; set; }
        public string InvoiceAddressLine1 { get; set; }
        public string InvoiceAddressLine2 { get; set; }
        public string InvoiceAddressLine3 { get; set; }
        public string InvoiceAddressLine4 { get; set; }
        public string InvoiceAddressLine5 { get; set; }
        public string Terms { get; set; }
        public bool OnHold { get; set; }
        public string CreditLimit { get; set; }

        public string GetId()
        {
            return AccountNumber;
        }

        public override string ToString()
        {
            return $"A/C:{AccountNumber}, Name:{Name}";
        }
    }
}
