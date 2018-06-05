using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Growflo.Integration.Windows
{
    public class WaitCursor : IDisposable
    {
        private Form _form;

        public WaitCursor(Form form)
        {
            _form = form;
            _form.Cursor = Cursors.WaitCursor;
        }

        public void Dispose()
        {
            _form.Cursor = Cursors.Default;
        }
    }
}
