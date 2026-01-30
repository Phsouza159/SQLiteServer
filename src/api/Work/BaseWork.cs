using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SQLiteServer.Work
{
    internal abstract class BaseWork : BackgroundService
    {
        internal abstract void ConfigurarDependencias(IServiceScope scope);

        protected T RecuperarServico<T>(IServiceScope scope)
        {
            return scope.ServiceProvider.GetService<T>()
                 ?? throw new ArgumentException($"Falha ao criar serviço '{typeof(T).Name}'");
        }
    }
}
