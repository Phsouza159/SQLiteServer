using SQLiteServer.Data.Enum;

namespace SQLiteServer.Data.Interface
{
    public interface ISQLiteService : IServicoBase
    {
        void AdicionarItemFila(IEntidade entidade);
    }
}
