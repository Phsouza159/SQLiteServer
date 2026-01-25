using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using SQLiteServer.Data;
using SQLiteServer.Data.Enum;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Services
{
    internal class kafkaServices : IkafkaServices
    {
        public kafkaServices(ILogger<kafkaServices> logger)
        {
            Logger = logger;
        }

        public ILogger<kafkaServices> Logger { get; }

       // public Queue<RegistroFila> Queue { get; private set; } = new Queue<RegistroFila>();

        private StatusKafkaService _status { get; set; }

        public StatusKafkaService Status { get => this._status; }

        public async Task Consumer(CancellationToken cancellationToken)
        {
            if (!Configuracao.STATUS_KAFKA_LISTEN)
                return;

            this._status = StatusKafkaService.START;

            var consumer =
                new ConsumerBuilder<Ignore, string>(this.RecuperarConfiguracao())
                    .SetErrorHandler(ErrorHandler)
                    .Build();

            string topico = Configuracao.KAFKA_TOPICO;
            consumer.Subscribe(topico);

            this.Logger.LogInformation("Conectando servico no topico: {0}", topico);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    this._status = StatusKafkaService.CONNECTED;
                    var consumeResult = consumer.Consume(cancellationToken);

                    //RegistroFila registro = new()
                    //{
                    //    Offset = consumeResult.Offset.Value,
                    //    Topico = consumeResult.Topic,
                    //    DataCadastro = consumeResult.Message.Timestamp,
                    //    Mensagem = consumeResult.Message.Value,
                    //    Status = StatusRegistroFila.PENDENTE
                    //};

                    //this.Queue.Enqueue(registro);
                    //this.Logger.LogInformation("Item adicionado na fila ID: {0}", registro.Offset);

                }
                catch (KafkaException e)
                {
                    this.Logger.LogError("Erro ao adicionar item na fila: {0}", e.Error.Reason);
                    this._status = StatusKafkaService.ERRO;
                    break;
                }
            }
        }

        private void ErrorHandler(IConsumer<Ignore, string> consumer, Error error)
        {
            this.Logger.LogError("Erro interno fila: {0}", error.Reason);
            this._status = StatusKafkaService.ERRO;

            //consumer.Close();
        }

        private IEnumerable<KeyValuePair<string, string>> RecuperarConfiguracao()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = Configuracao.KAFKA_SERVER,
                GroupId          = Configuracao.KAFKA_GRUPO,
                AutoOffsetReset  = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                MaxPollIntervalMs = 300000,
                SessionTimeoutMs  = 10000,
            };

            return config;
        }
    }
}
