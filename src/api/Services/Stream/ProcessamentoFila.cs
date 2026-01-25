using Microsoft.Extensions.Logging;
using SQLiteServer.Application.Server.Job;
using SQLiteServer.Data.Interface;
using System.Threading.Channels;

namespace SQLiteServer.Services.Stream
{
    internal class ProcessamentoFila<TSource> : IProcessamentoFila<TSource>
        where TSource : class
    {
        private readonly Channel<object> _fila;


        public ProcessamentoFila(ILogger<ProcessamentoFila<TSource>> logger)
        {
            _fila = Channel.CreateUnbounded<object>();
            Logger = logger;
        }

        public ILogger<ProcessamentoFila<TSource>> Logger { get; }

        public Task<TSource> Addicionar(Func<Task<TSource>> task)
        {
            var itemFila = new ItemFila<TSource>(task);
            _fila.Writer.WriteAsync(itemFila);
            return itemFila.Tcs.Task;
        }

        public async Task ProcessarFila(CancellationToken cancellationToken)
        {
            await foreach (var item in _fila.Reader.ReadAllAsync(cancellationToken))
            {
                try
                {
                    switch (item)
                    {
                        case ItemFila<object> itemObje:
                            await ExecuteJob(itemObje, cancellationToken);
                            break;

                        default:
                            // Usa reflexão para tipos genéricos
                            var type = item.GetType();
                            var method = GetType().GetMethod(nameof(ExecuteJob), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            var generic = method.MakeGenericMethod(type.GenericTypeArguments[0]);
                            await (Task)generic.Invoke(this, new[] { item , cancellationToken });
                            break;
                    }
                }
                catch (Exception ex)
                {
                    this.Logger.LogError(ex, "Erro falta - Processamento fila TCP.");
                }
            }
        }

        private async Task ExecuteJob<TSource>(ItemFila<TSource> item, CancellationToken cancellationToken)
            where TSource : class
        {
            try
            {
                var result = await item.Action();
                item.Tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                item.Tcs.SetException(ex);
            }
        }

    }
}
