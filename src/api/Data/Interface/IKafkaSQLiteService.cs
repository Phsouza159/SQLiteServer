using SQLiteServer.Data.Enum;

namespace SQLiteServer.Data.Interface
{
    public interface IKafkaSQLiteService
    {
        Task ProcessarFila(CancellationToken cancellationToken);

        void Conectar(IkafkaServices services);
    }
}
