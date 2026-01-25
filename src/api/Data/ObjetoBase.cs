using Newtonsoft.Json;
using SQLite;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Data
{
    public class ObjetoBase : IEntidade, IDisposable
    {
        public virtual Guid ID { get; set; }

        [Ignore]
        public string CaminhoArquivoBackup { get => Path.Combine(Configuracao.DIRETORIO_PASTA_TEMPORARIA, $"{this.ID}-registro.temp"); }

        public async Task SaveBackup(CancellationToken cancellationToken)
        {
            // GERAR CARGA BAKCUP
            await File.WriteAllTextAsync(this.CaminhoArquivoBackup, JsonConvert.SerializeObject(this), cancellationToken);
        }

        public void DeleteBackup()
        {
            if (File.Exists(this.CaminhoArquivoBackup))
                File.Delete(this.CaminhoArquivoBackup);
        }

        public bool IsDisposed { get; private set; }  

        public void Dispose()
        {
            if(this.IsDisposed)
            {
                GC.SuppressFinalize(this);
                this.IsDisposed = true;
            }
        }
    }
}
