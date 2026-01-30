using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Work
{
    internal class SQLiteWork : BaseWork
    {
        public SQLiteWork(
              ILogger<SQLiteWork> logger
            , IServiceScopeFactory serviceScopeFactory
            )
        {
            this.Logger = logger;
            this.ServiceScopeFactory = serviceScopeFactory;
            this.ServicosRegistrados = new List<IServicoBase>();
        }

        #region PROPRIEDADES

        private readonly ILogger<SQLiteWork> Logger;
        
        internal IServiceScopeFactory ServiceScopeFactory { get; set; }
        
        internal IkafkaServices kafkaServices { get; set; }

        internal ISQLiteService SQLiteService { get; set; }

        internal List<IServicoBase> ServicosRegistrados { get; set; }

        internal IProcessamentoFila ProcessamentoFilaTcp { get; set; }

        internal IServicoTcp ServicoTcp { get; set; }

        #endregion

        #region PRINCIPAL

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Serviço SQLiteWork executando em: {time}", DateTimeOffset.Now);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Serviço SQLiteWork encerrando em: {time}", DateTimeOffset.Now);

            await Application.DataCache.SalvarCacache();

            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = this.ServiceScopeFactory.CreateScope();
                this.ConfigurarDependencias(scope);

                // EXECUTAR SERVICO LISTEN DO KAFKA
                var kafakTask = this.kafkaServices.Consumer(cancellationToken);

                // EXECUTAR SERIVCO TCP
                var servicoTcp = this.ServicoTcp.Start(cancellationToken);

                // EXECUTAR FILA TCP 
                var tcpTask = this.ProcessamentoFilaTcp.ProcessarFila(cancellationToken);

                // EXECUCAO SERVICOS REGISTRADOS 
                var servicosRegistrados = this.ExecutarServicosRegistrados(cancellationToken);

                // EXECUTAR TODAS AS TASKS
                await Task.WhenAll(
                      kafakTask
                    , servicoTcp
                    , tcpTask
                    , servicosRegistrados
                );
            }
            catch(Exception ex)
            {
                this.Logger.LogError(ex, "Erro faltal no processo.");
            }
        }

        #endregion

        #region EXECUCAO SERVICOS 

        private async Task ExecutarServicosRegistrados(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // LISTA DE SERVICOS REGISTRADO
                foreach (var servico in this.ServicosRegistrados)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // EXECUTAR SERVICO REGISTRADO
                    _ = await servico.ExecutarServico(cancellationToken);
                }

                // DELAY DE EXECUCAO - 15 s
                await Task.Delay(1000 * 15, cancellationToken);
            }
        }

        #endregion

        #region EXECUCAO DEPENDENCIAS

        internal override void ConfigurarDependencias(IServiceScope scope)
        {
            this.kafkaServices          = RecuperarServico<IkafkaServices>(scope);
            this.SQLiteService          = RecuperarServico<ISQLiteService>(scope);
            this.ProcessamentoFilaTcp   = RecuperarServico<IProcessamentoFila>(scope);
            this.ServicoTcp             = RecuperarServico<IServicoTcp>(scope);
           
            this.ServicosRegistrados.Add(SQLiteService);
        }

        #endregion
    }
}
