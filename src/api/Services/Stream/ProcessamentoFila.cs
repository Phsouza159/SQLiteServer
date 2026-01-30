using Microsoft.Extensions.Logging;
using SQLiteServer.Application;
using SQLiteServer.Application.Server.Job;
using SQLiteServer.Data.Interface;
using SQLiteServer.Data.Registros;

namespace SQLiteServer.Services.Stream
{
    internal class ProcessamentoFila : IProcessamentoFila
    {
        public ProcessamentoFila(ILogger<ProcessamentoFila> logger)
        {
            Logger = logger;
        }

        public ILogger<ProcessamentoFila> Logger { get; }

        public async Task ProcessarFila(CancellationToken cancellationToken)
        {
            await foreach (var item in DataCache.FilaRegistroTcp.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    switch (item)
                    {
                        case ItemFila<RegistroTcpSQLite> itemConcreto:
                            await ExecuteJob(itemConcreto, cancellationToken);
                            break;

                        case ItemFila<IRegistroTcpSQLite> itemInterface:
                            await ExecuteJob(itemInterface, cancellationToken);
                            break;

                        default:
                            throw new InvalidOperationException(
                                $"Tipo de ItemFila não suportado: {item.GetType()}"
                            );
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Erro falta - Processamento fila TCP.");
                }
            }
        }

        private async Task ExecuteJob<T>(ItemFila<T> item, CancellationToken cancellationToken)
            where T : IEntidade
        {
            try
            {
                var result = await item.FilaOperacao();
                item.FonteConclusaoTarefa.SetResult(result);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Erro execução operação Fila.");
                item.FonteConclusaoTarefa.SetException(ex);
            }
        }
    }
}
