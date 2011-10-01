using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LittleNet.NDecompile.FormsUI
{
    internal class WaitCursor : IDisposable
    {
        private Cursor _cursor;

        public WaitCursor()
        {
            _cursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Cursor.Current = _cursor;
        }

        #endregion
    }
}
