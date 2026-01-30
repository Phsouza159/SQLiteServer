namespace SQLiteServer.Application
{
    public static class Configuracao
    {

        #region RECUPERAR CONFIGURACAO

        private static Dictionary<string, string> Data { get; set; } = null;

        /// <summary>
        /// RECUPERAR DADOS DO ARQUIVO .ENV
        /// </summary>
        private static Dictionary<string, string> RecuperarValores()
        {
            if (Data is null)
            {
                Data = new Dictionary<string, string>();

                string diretorio = AppContext.BaseDirectory
                        ?? throw new ArgumentException("Falha ao carregar diretorio raiz.");

                string caminhoArquivoConfig = Path.Combine(diretorio, $".env");
                string[] valores = File.ReadAllLines(caminhoArquivoConfig);

                foreach (string valor in valores)
                {
                    if (string.IsNullOrEmpty(valor) || !valor.Contains("="))
                        throw new ArgumentException("Arquivo .env inválido.");

                    string itemChave = valor.Split("=")[0];
                    string imteValor = string.Join("=", valor.Split("=").Where((k, l) => l > 0));

                    if (Data.ContainsKey(itemChave))
                        throw new ArgumentException($"Chave duplicada para o arquivo de configuração '{itemChave}'.");

                    Data.Add(itemChave, imteValor);
                }
            }

            return Data;
        }

        #endregion

        private static string RecuperarValor(string chave, string valorDefault = "")
        {
            RecuperarValores();

            if (Data.TryGetValue(chave, out string? valor))
                return valor;

            throw new ArgumentException($"Chave não configurado '{chave}'");
        }

        private static void SetarValor(string chave, string valor)
        {
            if (Data.ContainsKey(chave))
            {
                Data[chave] = valor;
                return;
            }

            Data[chave] = valor;
        }

        /// <summary>
        /// CAMINHO DO ARQUIV SQLite do LOG
        /// </summary>
        public static string CAMINHO_ARQUIVO_LOG_DATABASE
        {
            get => RecuperarValor(nameof(CAMINHO_ARQUIVO_LOG_DATABASE));
        }

        /// <summary>
        /// URL do serviço KAFKA
        /// </summary>
        public static string KAFKA_SERVER
        {
            get => RecuperarValor(nameof(KAFKA_SERVER));
        }

        /// <summary>
        /// GRUPO KAFKA
        /// </summary>
        public static string KAFKA_GRUPO
        {
            get => RecuperarValor(nameof(KAFKA_GRUPO));
        }

        /// <summary>
        /// Registro KAFKA Topico
        /// </summary>
        public static string KAFKA_TOPICO
        {
            get => RecuperarValor(nameof(KAFKA_TOPICO));
        }

        /// <summary>
        /// Diretorio Temporario para arquivos
        /// </summary>
        public static string DIRETORIO_PASTA_TEMPORARIA
        {
            get => RecuperarValor(nameof(DIRETORIO_PASTA_TEMPORARIA));
            set => SetarValor(nameof(DIRETORIO_PASTA_TEMPORARIA), value);
        }

        public static bool STATUS_SQLITE_SERVICE
        {
            get => bool.Parse(RecuperarValor(nameof(STATUS_SQLITE_SERVICE)));
        }

        public static bool STATUS_KAFKA_LISTEN
        {
            get => bool.Parse(RecuperarValor(nameof(STATUS_KAFKA_LISTEN)));
        }
    }
}
