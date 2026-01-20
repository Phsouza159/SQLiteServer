using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Work
{
    public class SQLiteWork : BackgroundService
    {
        public SQLiteWork(
              ILogger<SQLiteWork> logger
            , IkafkaServices kafkaServices
            , IKafkaSQLiteService kafkaSQLiteService)
        {
            this.Logger = logger;
            this.kafkaServices = kafkaServices;
            this.kafkaSQLiteService = kafkaSQLiteService;
        }

        private readonly ILogger<SQLiteWork> Logger;
        private readonly IkafkaServices kafkaServices;
        private readonly IKafkaSQLiteService kafkaSQLiteService;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            this.Logger.LogInformation("Serviço SQLiteWork executando em: {time}", DateTimeOffset.Now);
            this.kafkaSQLiteService.Conectar(this.kafkaServices);

            // EXECUTAR WORK DE FILA KAFKA 
            _ = Task.Run(
                  () => this.kafkaServices.Consumer(cancellationToken),
                  cancellationToken
            );

            while (!cancellationToken.IsCancellationRequested)
            {
                // EXECUTAR FILA KAFKA SQLite
                await this.kafkaSQLiteService.ProcessarFila(cancellationToken);

                // 5000 / 1000 = 5 s
                await Task.Delay(5000, cancellationToken);
            }
        }
    }
}
