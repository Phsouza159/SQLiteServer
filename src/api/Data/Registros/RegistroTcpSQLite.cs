using SQLite;
using SQLiteServer.Data.Interface;

namespace SQLiteServer.Data.Registros
{
    [Table("RegistroLogsTcp")]
    public class RegistroTcpSQLite : ObjetoBase<Guid>, IRegistroTcpSQLite
    {
        public RegistroTcpSQLite()
        {
            this.ID = Guid.NewGuid();
            this.DataRegistro = DateTime.Now;
        }

        [PrimaryKey]
        [Column("Ticket")]
        public override Guid ID { get; set; }

        [Column("Timestamp")]
        public DateTime DataRegistro { get; set; }

        [Column("Mensagem")]
        public string Mensagem { get; set; } = string.Empty;
    }
}
