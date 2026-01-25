namespace SQLiteServer.Application.Server.Job
{
    internal class ItemFila<T>
        where T : class
    {
        public Func<Task<T>> Action { get; }
        public TaskCompletionSource<T> Tcs { get; }

        public ItemFila(Func<Task<T>> action)
        {
            Action = action;
            Tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
