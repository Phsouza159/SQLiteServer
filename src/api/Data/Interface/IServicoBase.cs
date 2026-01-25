namespace SQLiteServer.Data.Interface
{
    public interface IServicoBase
    {
        bool IsAtivo { get; }

        DateTime? ProximaExecucao { get; set;  }

        Task<bool> ExecutarServico(CancellationToken cancellationToken);
    }
}
