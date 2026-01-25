using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteServer.Data.Interface
{
    public interface IEntidade
    {
        string CaminhoArquivoBackup { get; }

        Task SaveBackup(CancellationToken cancellationToken);

        void DeleteBackup();
    }
}
