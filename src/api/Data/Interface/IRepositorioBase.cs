using System.Linq.Expressions;

namespace SQLiteServer.Data.Interface
{
    internal interface IRepositorioBase<T>
        where T : IEntidade, new()
    {
        Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate);

        Task<List<T>> Where(Expression<Func<T, bool>> predicate);
    }
}
