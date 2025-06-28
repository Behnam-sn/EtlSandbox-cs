using System.Data;

using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Shared;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly IDbConnectionFactory _connectionFactory;

    private IDbConnection? _connection;

    private IDbTransaction? _transaction;

    private bool _disposed;

    public UnitOfWork(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public IDbConnection Connection => _connection ??= _connectionFactory.CreateConnection();

    public IDbTransaction? Transaction => _transaction;

    public void BeginTransaction()
    {
        if (_transaction is null)
        {
            _transaction = Connection.BeginTransaction();
        }
    }

    public void Commit()
    {
        _transaction?.Commit();
        _transaction?.Dispose();
        _transaction = null;
    }

    public void Rollback()
    {
        _transaction?.Rollback();
        _transaction?.Dispose();
        _transaction = null;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _transaction?.Dispose();
        _connection?.Dispose();
        _disposed = true;
    }
}