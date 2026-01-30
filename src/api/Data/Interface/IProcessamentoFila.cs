namespace SQLiteServer.Data.Interface
{
    public interface IProcessamentoFila
    {
        Task ProcessarFila(CancellationToken cancellationToken);
    }
}
