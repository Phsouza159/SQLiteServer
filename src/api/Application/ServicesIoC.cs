using Microsoft.Extensions.DependencyInjection;
using SQLiteServer.Application.Repository;
using SQLiteServer.Data.Interface;
using SQLiteServer.Services;
using SQLiteServer.Services.Stream;

namespace SQLiteServer.Application
{
    internal static class ServicesIoC
    {
        internal static void ConfigurarIoC(this IServiceCollection services)
        {
            services.AddScoped<IkafkaServices, kafkaServices>();
            services.AddScoped<ISQLiteService, SQLiteService>();
            services.AddScoped<IServicoTcp, ServicoTcp>();

            services.AddScoped<IProcessamentoFila, ProcessamentoFila>();
            services.AddScoped(typeof(IRepositorioBase<>), typeof(RepositorioBase<>));
        }
    }
}
