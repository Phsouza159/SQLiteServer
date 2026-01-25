namespace SQLiteServer.Data.Interface
{
    public interface IProcessamentoFila<TSource>
        where TSource : class
    {
        Task<TSource> Addicionar(Func<Task<TSource>> task);

        Task ProcessarFila(CancellationToken cancellationToken);
    }
}
