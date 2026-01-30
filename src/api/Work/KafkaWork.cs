using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SQLiteServer.Data;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Work
{
    internal class KafkaWork : BaseWork
    {
        public KafkaWork(
           ILogger<KafkaWork> logger
         , IServiceScopeFactory serviceScopeFactory
         )
        {
            this.Logger = logger;
            this.ServiceScopeFactory = serviceScopeFactory;
        }

        #region PROPRIEDADES

        private readonly ILogger<KafkaWork> Logger;

        internal IkafkaServices kafkaServices { get; set; }
        
        internal IRepositorioBase<RegistroKafka> Repositorio { get; set; }

        internal IServiceScopeFactory ServiceScopeFactory { get; set; }

        #endregion

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Serviço KafkaWork executando em: {time}", DateTimeOffset.Now);
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Serviço KafkaWork encerrando em: {time}", DateTimeOffset.Now);
            await base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = this.ServiceScopeFactory.CreateScope();
                this.ConfigurarDependencias(scope);

                string nome = "";

                var registro = await this.Repositorio
                        .FirstOrDefault(e => e.Nome.ToUpper().Equals(nome.ToUpper()))
                        ?? throw new ArgumentException($"Registro não localizado '{nome}'.");

                while (!cancellationToken.IsCancellationRequested)
                {
                    // EXECUTAR SERVICO LISTEN DO KAFKA
                    await this.kafkaServices.Consumer(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Erro faltal no processo.");
            }
        }

        #region EXECUCAO DEPENDENCIAS

        internal override void ConfigurarDependencias(IServiceScope scope)
        {
            this.kafkaServices = RecuperarServico<IkafkaServices>(scope);
            this.Repositorio = RecuperarServico<IRepositorioBase<RegistroKafka>>(scope);
        }

        #endregion
    }
}
