using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteServer.Data.Interface
{
    public interface IkafkaServices
    {
        Queue<RegistroFila> Queue { get; }

        void Consumer(CancellationToken cancellationToken);
    }
}
