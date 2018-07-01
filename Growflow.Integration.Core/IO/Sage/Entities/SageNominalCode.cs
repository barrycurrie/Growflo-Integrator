using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Sage.Entities
{
    public class SageNominalCode : ISageEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string GetId()
        {
            return Code;
        }

        public override string ToString()
        {
            return $"Code:{Code}, Name:{Name}";
        }
    }
}
