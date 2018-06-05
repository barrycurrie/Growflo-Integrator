using Growflo.Integration.Core.Sage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Growflo.Integration.Woodlark
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ISageController sageController = new Sage24Controller();

            WorkflowController workflow = new WorkflowController(sageController);
            workflow.Execute();

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Mainform());
        }


    }
}
