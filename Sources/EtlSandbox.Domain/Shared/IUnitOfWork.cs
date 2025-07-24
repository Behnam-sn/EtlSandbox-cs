using System.Data;

namespace EtlSandbox.Domain.Shared;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IDbConnection Connection { get; }
}