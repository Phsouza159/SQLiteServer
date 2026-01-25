using SQLiteServer.Data.Enum;

namespace SQLiteServer.Data.Interface
{
    public interface IkafkaServices
    {
        StatusKafkaService Status { get; }

        Task Consumer(CancellationToken cancellationToken);
    }
}
