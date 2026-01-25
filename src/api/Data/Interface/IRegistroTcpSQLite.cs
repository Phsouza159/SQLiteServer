using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteServer.Data.Interface
{
    public interface IRegistroTcpSQLite : IEntidade
    {
        Guid ID { get; set; }
        
        DateTime DataRegistro { get; set; }
        
        string Mensagem { get; set; }
    }
}
