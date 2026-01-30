using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteServer.Data.Interface
{
    internal interface IServicoTcp
    {
        Task Start(CancellationToken cancellationToken);
    }
}
