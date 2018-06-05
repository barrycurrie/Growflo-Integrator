using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Sage
{
    public class SageConnectionFailedException : Exception
    {
        public SageConnectionFailedException() :
            base("Unable to connect to sage.\n\rPlease verify the sage username, password and data path.")
        {

        }

    }

    public class NotConnectedException : Exception
    {
        public NotConnectedException() : 
            base("Not connected to sage 50")
        {

        }

    }

    public class CustomerNotFoundException : Exception
    {
        public CustomerNotFoundException(string accountRef) :
            base($"{accountRef} not a valid account number.")
        {

        }
    }
}
