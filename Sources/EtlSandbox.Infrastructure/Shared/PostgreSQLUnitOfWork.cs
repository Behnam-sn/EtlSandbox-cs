using System.Data;

using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

using Npgsql;

namespace EtlSandbox.Infrastructure.Shared;

public sealed class PostgreSQLUnitOfWork : IUnitOfWork
{
    private readonly string _connectionString;

    private NpgsqlConnection? _connection;

    private NpgsqlTransaction? _transaction;

    private bool _disposed;

    public IDbConnection Connection => _connection ?? throw new InvalidOperationException("Connection is not initialized.");

    public IDbTransaction? Transaction => _transaction;

    public PostgreSQLUnitOfWork(IOptions<DatabaseConnections> options)
    {
        _connectionString = options.Value.SqlServer;
    }

    public async Task OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
        }

        if (_connection.State != ConnectionState.Open)
        {
            await _connection.OpenAsync(cancellationToken);
        }
    }

    public void BeginTransaction()
    {
        if (_connection == null)
            throw new InvalidOperationException("Connection must be opened before beginning a transaction.");
        _transaction = _connection.BeginTransaction();
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