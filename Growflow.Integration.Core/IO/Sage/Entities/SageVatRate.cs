using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Sage.Entities
{
    public class SageVatRate : ISageEntity
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public decimal Value { get; set; }

        public string GetId()
        {
            return Code;
        }

        public override string ToString()
        {
            return $"Code:{Code}, Description:{Description}, Value:{Value}";
        }
    }
}
