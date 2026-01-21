
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
            //Log.Logger = new LoggerConfiguration()
            //   // .WriteTo.Console() PARA DEBUG
            //    .WriteTo.SQLite(sqliteDbPath: "logs.db", tableName: "SysLogs")
            //    .CreateLogger();

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
                        .WriteTo.SQLite(sqliteDbPath: "C:\\_git\\SQLiteServer\\src\\api\\log.db", tableName: "SysLogs") 

               )
               .ConfigureServices((hostContext, services) =>
               {
                   services.ConfigurarIoC();

                   services.AddHostedService<SQLiteWork>();
               });
        }
    }
}
