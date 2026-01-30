using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SQLiteServer.Application;
using SQLiteServer.Data.Interface;
using SQLiteServer.Data.Registros;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SQLiteServer.Services.Stream
{
    public class ServicoTcp : IServicoTcp
    {
        public ServicoTcp(ILogger<ServicoTcp> logger)
        {
            Logger = logger;
        }

        internal readonly ILogger<ServicoTcp> Logger;

        public async Task Start(CancellationToken cancellationToken)
        {
            try
            {
                int port = 50123;
                TcpListener server = new TcpListener(IPAddress.Any, port);

                server.Start();
                this.Logger.LogInformation("Servico TCP online na porta {0}", port);

                this.ConfiguraPastaTemporia();

                while (!cancellationToken.IsCancellationRequested)
                {
                    TcpClient client = await server.AcceptTcpClientAsync(cancellationToken);
                    client.SendTimeout = 1000 * 60 * 60;
                    client.ReceiveTimeout = 3000;

                    NetworkStream stream = client.GetStream();

                    var registroTcp = await DataCache.AddRegistroFilaTcp(async () =>
                    {
                        IRegistroTcpSQLite registro = new RegistroTcpSQLite();
                        byte[] buffer = await this.CarregarStream(stream);
                        registro.Mensagem = Encoding.ASCII.GetString(buffer, 0, buffer.Length);

                        DataCache.FilaProcessamentoSQLite.Enqueue(registro);
                        return registro;
                    });

                    await stream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(registroTcp)), cancellationToken);
                    Console.WriteLine($"Recebido: {registroTcp.Mensagem}");

                    client.Close();
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "Erro interno no serviço TCP.");
            }
        }

        private void ConfiguraPastaTemporia()
        {
            string diretorio = Path.Combine(AppContext.BaseDirectory, "temp");
            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }

            Configuracao.DIRETORIO_PASTA_TEMPORARIA = diretorio;
        }

        private async Task<byte[]> CarregarStream(NetworkStream stream)
        {
            using var ms = new MemoryStream();
            byte[] buffer = new byte[1024];

            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            ms.Write(buffer, 0, bytesRead);

            return ms.ToArray();
        }
    }
}
