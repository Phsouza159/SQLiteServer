using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteServer.Data
{
    internal class ObjetoBase : IDisposable
    {

        public bool IsDisposed { get; private set; }  

        public void Dispose()
        {
            if(this.IsDisposed)
            {
                GC.SuppressFinalize(this);
                this.IsDisposed = true;
            }
        }
    }
}
