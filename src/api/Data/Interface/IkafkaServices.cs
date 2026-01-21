using SQLiteServer.Data.Enum;

namespace SQLiteServer.Data.Interface
{
    public interface IkafkaServices
    {
        StatusKafkaService Status { get; }

        Queue<RegistroFila> Queue { get; }

        void Consumer(CancellationToken cancellationToken);
    }
}
