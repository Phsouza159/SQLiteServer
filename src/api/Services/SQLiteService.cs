using Microsoft.Extensions.Logging;
using SQLite;
using SQLiteServer.Data;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Services
{
    public class SQLiteService : ISQLiteService
    {
        public SQLiteService(ILogger<SQLiteService> logger)
        {
            this.Logger = logger;
            this.fila = new Queue<IEntidade>();
        }

        private ILogger<SQLiteService> Logger { get; }

        private readonly Queue<IEntidade> fila;

        public bool IsAtivo => Configuracao.STATUS_SQLITE_SERVICE;

        public DateTime? ProximaExecucao { get; set;  }

        public async Task<bool> ExecutarServico(CancellationToken cancellationToken)
        {
            try
            {
                // EXECUCAO ONLISE DA FILA
                this.ProximaExecucao = DateTime.Now;

                return await this.ProcessarFila(cancellationToken);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, $"Erro interno ao processa fila: '{nameof(SQLiteService)}'");
                return false;
            }
        }

        #region REGISTRA ITEM NA FILA

        public void AdicionarItemFila(IEntidade entidade)
        {
            this.fila.Enqueue(entidade);
        }

        #endregion

        #region EXECUTAR SERVICO

        public async Task<bool> ProcessarFila(CancellationToken cancellationToken)
        {
            if(this.fila.Any())
            {
                SQLitePCL.Batteries_V2.Init();

                var options = new SQLiteConnectionString(Configuracao.CAMINHO_ARQUIVO_LOG_DATABASE, false);
                var conexao = new SQLiteAsyncConnection(options);

                await this.ProcessarFila(conexao, cancellationToken);

                return true;
            }

            return false;
        }

        private async Task ProcessarFila(
              SQLiteAsyncConnection conexao
            , CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested || !this.fila.Any())
                return;

            var registro = this.fila.Dequeue();

            await registro.SaveBackup(cancellationToken);

            int row = await this.ExecutarRegistro(conexao, registro, cancellationToken);

            if (row > 0)
                registro.DeleteBackup();

            await this.ProcessarFila(conexao, cancellationToken);
        }

        private async Task<int> ExecutarRegistro(SQLiteAsyncConnection conexao, IEntidade registro, CancellationToken cancellationToken)
        {
            return await conexao.InsertAsync(registro);
        }

        #endregion
    }
}
