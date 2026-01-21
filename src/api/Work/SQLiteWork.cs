using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Work
{
    public class SQLiteWork : BackgroundService
    {
        public SQLiteWork(
              ILogger<SQLiteWork> logger
            , IServiceScopeFactory serviceScopeFactory
            )
        {
            this.Logger = logger;
            this.ServiceScopeFactory = serviceScopeFactory;
        }

        internal ILogger<SQLiteWork> Logger { get; set; }
        internal IServiceScopeFactory ServiceScopeFactory { get; set; }
        internal IkafkaServices kafkaServices { get; set; }
        internal IKafkaSQLiteService kafkaSQLiteService { get; set; }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = this.ServiceScopeFactory.CreateScope();

                this.kafkaServices = scope.ServiceProvider.GetService<IkafkaServices>()
                        ?? throw new ArgumentException("Falha ao criar serviço 'IkafkaServices'");
                this.kafkaSQLiteService = scope.ServiceProvider.GetService<IKafkaSQLiteService>()
                        ?? throw new ArgumentException("Falha ao criar serviço 'IKafkaSQLiteService'");

                this.Logger.LogInformation("Serviço SQLiteWork executando em: {time}", DateTimeOffset.Now);
                this.kafkaSQLiteService.Conectar(this.kafkaServices);

                // EXECUTAR WORK DE FILA KAFKA 
                _ = Task.Run(
                      () => this.kafkaServices.Consumer(cancellationToken),
                      cancellationToken
                );

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (this.kafkaServices.Status == Data.Enum.StatusKafkaService.ERRO)
                        break;

                    // EXECUTAR FILA KAFKA SQLite
                    await this.kafkaSQLiteService.ProcessarFila(cancellationToken);

                    // 5000 / 1000 = 5 s
                    await Task.Delay(5000, cancellationToken);
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
}
