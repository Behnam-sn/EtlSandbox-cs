using System.Data;

using EtlSandbox.Domain.Shared;

namespace EtlSandbox.Infrastructure.Shared.UnitOfWorks;

public sealed class RawSqlUnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory;

    private IDbConnection? _connection;

    private bool _disposed;

    public RawSqlUnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IDbConnection Connection => _connection ??= _connectionFactory.CreateConnection();

    public void Dispose()
    {
        if (_disposed) return;
        _connection?.Dispose();
        _disposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is IAsyncDisposable connectionAsyncDisposable)
        {
            await connectionAsyncDisposable.DisposeAsync();
        }
        else
        {
            _connection?.Dispose();
        }
    }
}