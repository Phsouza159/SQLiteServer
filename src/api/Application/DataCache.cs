using SQLiteServer.Application.Server.Job;
using SQLiteServer.Data.Interface;
using System.Threading.Channels;

namespace SQLiteServer.Application
{
    internal static class DataCache
    {
        private static Channel<object>? _filaRegistroTcp { get; set; } = null;

        private static Queue<IEntidade>? _filaProcessamentoSQLite { get; set; } = null;

        public static Channel<object> FilaRegistroTcp
        {
            get
            {
                if (_filaRegistroTcp is null)
                    _filaRegistroTcp = Channel.CreateUnbounded<object>();

                return _filaRegistroTcp;
            }
        }

        public static Queue<IEntidade> FilaProcessamentoSQLite
        {
            get
            {
                if (_filaProcessamentoSQLite is null)
                    _filaProcessamentoSQLite = new Queue<IEntidade>();

                return _filaProcessamentoSQLite;
            }
        }

        #region ADICIONAR REGISTRO FILA 

        public static Task<TSource> AddRegistroFilaTcp<TSource>(Func<Task<TSource>> registroTcp)
            where TSource : IRegistroTcpSQLite
        {
            var itemFila = new ItemFila<TSource>(registroTcp);
            FilaRegistroTcp.Writer.WriteAsync(itemFila);
            return itemFila.FonteConclusaoTarefa.Task;
        }

        #endregion

        #region SALVAR CACHE

        internal static async Task SalvarCacache()
        {
            if(DataCache._filaProcessamentoSQLite is not null
                && DataCache._filaProcessamentoSQLite.Any())
            {
                CancellationToken cancellationToken = CancellationToken.None;

                foreach (var entidade in DataCache._filaProcessamentoSQLite)
                {
                   await entidade.SaveBackup(cancellationToken);
                }
            }
        }

        #endregion
    }
}
