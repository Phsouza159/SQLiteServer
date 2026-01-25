using Microsoft.Extensions.DependencyInjection;
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

            
            services.AddSingleton(typeof(IProcessamentoFila<IRegistroTcpSQLite>), typeof(ProcessamentoFila<IRegistroTcpSQLite>));
        }
    }
}
