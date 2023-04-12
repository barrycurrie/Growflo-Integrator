using System;
using System.Collections.Generic;

namespace Growflo.Integration.Core.Sage.Entities
{
    public class SageBatchInvoice : ISageEntity
    {
        public int? SageInvoiceNo { get; set; }
        public string InvoiceReference { get; set; }
        public string CustomerAccountNumber { get; set; }
        public DateTime Date { get; set; }
        public bool IsCredit { get; set; }
        public string InvoiceType {  get { return IsCredit ? "Credit" : "Invoice"; } }
        public List<Split> Splits { get; set; } = new List<Split>();
        public string Currency { get; set; }
        public string Name { get; internal set; }
        public string Details { get; set; }

        public string GetId()
        {
            return SageInvoiceNo.HasValue ? SageInvoiceNo.ToString() : "";
        }

        public override string ToString()
        {
            return $"Invoice No:{SageInvoiceNo}; Growflo No:{InvoiceReference}; A/C:{CustomerAccountNumber}";
        }

        public class Split
        {
            public string NominalCode { get; set; }
            public string VatCode { get; set; }
            public double NetAmount { get; set; }
            public double VatAmount { get; set; }
            public string Details { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
