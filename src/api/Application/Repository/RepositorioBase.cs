using SQLite;
using SQLiteServer.Data;
using SQLiteServer.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteServer.Application.Repository
{
    internal class RepositorioBase<T> : IRepositorioBase<T>
        where T : IEntidade, new()
    {
        public async Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            var options = new SQLiteConnectionString(Configuracao.CAMINHO_ARQUIVO_LOG_DATABASE, false);
            var conexao = new SQLiteAsyncConnection(options);

            return await conexao
                            .Table<T>()
                            .FirstOrDefaultAsync(predicate);
        }


        public async Task<List<T>> Where(Expression<Func<T, bool>> predicate)
        {
            var options = new SQLiteConnectionString(Configuracao.CAMINHO_ARQUIVO_LOG_DATABASE, false);
            var conexao = new SQLiteAsyncConnection(options);

            return await conexao
                            .Table<T>()
                            .Where(predicate)
                            .ToListAsync()
                            .ConfigureAwait(false);
        }
    }
}
