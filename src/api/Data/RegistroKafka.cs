using SQLite;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Data
{
    [Table("RegistroKafka")]
    internal class RegistroKafka : ObjetoBase<int>, IEntidade
    {
        public RegistroKafka()
        {
        }

        [PrimaryKey]
        [Column("ID")]
        public override int ID { get; set; }

        [Column("Nome")]
        public string Nome { get; set; } = string.Empty;

        [Column("Server")]
        public string Server { get; set; } = string.Empty;

        [Column("GroupID")]
        public string GroupID { get; set; } = string.Empty;

        [Column("FlagAtivo")]
        public int FlagAtivo { get; set; }
    }
}
