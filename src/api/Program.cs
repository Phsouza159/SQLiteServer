
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SQLiteServer.Application;
using SQLiteServer.Work;
using Serilog;

namespace SQLiteServer
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var app = CreateHostBuilder(args).Build();

            await app.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            // Configure the Serilog logger
            Log.Logger = new LoggerConfiguration()
               // .WriteTo.Console() PARA DEBUG
                .WriteTo.SQLite(sqliteDbPath: "logs.db", tableName: "Logs")
                .CreateLogger();

           return Host.CreateDefaultBuilder(args)
               .UseWindowsService()
               .UseSerilog()
               .ConfigureServices((hostContext, services) =>
               {
                   services.ConfigurarIoC();

                   services.AddHostedService<SQLiteWork>();
               });
        }
    }
}
