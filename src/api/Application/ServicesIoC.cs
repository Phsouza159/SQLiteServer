using Microsoft.Extensions.DependencyInjection;
using SQLiteServer.Data.Interface;
using SQLiteServer.Services;

namespace SQLiteServer.Application
{
    internal static class ServicesIoC
    {
        internal static void ConfigurarIoC(this IServiceCollection services)
        {
            services.AddScoped<IkafkaServices, kafkaServices>();
            services.AddScoped<IKafkaSQLiteService, KafkaSQLiteService>();
        }
    }
}
