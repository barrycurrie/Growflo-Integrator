using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Sage.Generic
{
    public class SageSdoEngine<T> : ISageWorkSpace
        where T : class
    {
        public bool Connect(string username, string password, ref string errorMessage)
        {
            
        }
    }
}
