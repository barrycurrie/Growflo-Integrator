using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Growflo.Integration.Core.Sage.Generic
{
    public class GenericSageController
    {
        public bool Connect(string username, string password, ref string errorMessage)
        {
            try
            {
                if (!Directory.Exists(SageDataPath))
                {
                    throw new DirectoryNotFoundException(
                        $"The specified sage data path does not exist: {SageDataPath}");
                }

                _engine = new SDOEngine();
                _workSpace = (WorkSpace)_engine.Workspaces.Add("GROWFLO");

                IsConnected = _workSpace.Connect(SageDataPath, username, password);
            }
            catch (Exception)
            {
                IsConnected = false;
                errorMessage = _engine.LastError.Text;
            }

            return IsConnected;
        }

    }
}
