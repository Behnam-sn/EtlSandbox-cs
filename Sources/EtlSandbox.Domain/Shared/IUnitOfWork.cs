using System.Data;

namespace EtlSandbox.Domain.Shared;

public interface IUnitOfWork : IDisposable
{
    IDbConnection Connection { get; }

    IDbTransaction? Transaction { get; }

    Task OpenConnectionAsync(CancellationToken cancellationToken = default);

    void BeginTransaction();

    void Commit();

    void Rollback();
}