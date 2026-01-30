using SQLiteServer.Data.Interface;

namespace SQLiteServer.Application.Server.Job
{
    internal class ItemFila<T>
        where T : IEntidade
    {
        public Func<Task<T>> FilaOperacao { get; }
        public TaskCompletionSource<T> FonteConclusaoTarefa { get; }

        public ItemFila(Func<Task<T>> filaOperacao)
        {
            FilaOperacao = filaOperacao;
            FonteConclusaoTarefa = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
