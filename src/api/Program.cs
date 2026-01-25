
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SQLiteServer.Application;
using SQLiteServer.Work;
using Serilog;
using SQLiteServer.Data;
using SQLiteServer.Services.Stream;

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
            SQLitePCL.Batteries_V2.Init();

            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "serilog-errors.txt"), msg);
            });

            return Host.CreateDefaultBuilder(args)
               .UseWindowsService()
               .UseSerilog(
                    (context, services, configuration) => configuration
                      //  .ReadFrom.Configuration(context.Configuration) 
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()
                        .WriteTo.Console() // PARA DEBUG
                        .WriteTo.SQLite(
                              sqliteDbPath: Configuracao.CAMINHO_ARQUIVO_LOG_DATABASE
                            , tableName: "SysLogs"
                        ) 

               )
               .ConfigureServices((hostContext, services) =>
               {
                   services.ConfigurarIoC();

                   services.AddHostedService<SQLiteWork>();
               });
        }
    }
}
