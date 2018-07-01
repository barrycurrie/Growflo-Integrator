using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Growflo.Integration.Core.Controllers;
using Growflo.Integration.Core.Entities;
using Growflo.Integration.Core.Entities.Web;

namespace Growflow.Integration.Quantil.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            WebController webController = new WebController();
            var customers = webController.DownloadCustomers();

            foreach (OnlineCustomer customer in customers)
            {
                var sageCustomer = new MappingHelper().CreateSageCustomer(customer);

            }
        }
    }
}
