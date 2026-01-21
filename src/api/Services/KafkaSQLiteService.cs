using Microsoft.Extensions.Logging;
using SQLiteServer.Data;
using SQLiteServer.Data.Enum;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Services
{
    public class KafkaSQLiteService : IKafkaSQLiteService
    {
        public KafkaSQLiteService(ILogger<KafkaSQLiteService> logger)
        {
            Logger = logger;
        }

        internal ILogger<KafkaSQLiteService> Logger { get; }

        internal bool IsConectado { get; private set; }

        private IkafkaServices kafkaServices { get; set; }

        public async Task ProcessarFila(CancellationToken cancellationToken)
        {
            if (!this.IsConectado)
                throw new ArgumentException("Serviço não conectado ao topico kafka.");

            await this.ProcessarFilaKafka(this.kafkaServices.Queue, cancellationToken);
        }

        private async Task ProcessarFilaKafka(
              Queue<RegistroFila> queue
            , CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested || !queue.Any())
                return;

            var registro = queue.Dequeue();
            registro.Status = StatusRegistroFila.EM_EXECUCAO;

            await this.ExecutarRegistro(registro, cancellationToken);
            await this.ProcessarFilaKafka(queue, cancellationToken);
        }

        private async Task ExecutarRegistro(RegistroFila registro, CancellationToken cancellationToken)
        {
            // TODO - PROCESSAR REGISTRO PARA SQLite
        }

        #region CONECTAR SERVICO

        public void Conectar(IkafkaServices services)
        {
            this.IsConectado = true;
            this.kafkaServices = services;
        }

        #endregion
    }
}
