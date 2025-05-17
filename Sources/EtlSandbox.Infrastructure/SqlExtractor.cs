using Dapper;
using EtlSandbox.Domain;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace EtlSandbox.Infrastructure;

public sealed class SqlExtractor : IExtractor<CustomerOrderFlat>
{
    private readonly string _connectionString;
    private const int BatchSize = 100;

    public SqlExtractor(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IReadOnlyList<CustomerOrderFlat>> ExtractAsync(DateTime since, CancellationToken cancellationToken)
    {
        using var connection = new MySqlConnection(_connectionString);
        var sql = @"
            SELECT r.rental_id AS RentalId,
                   CONCAT(c.first_name, ' ', c.last_name) AS CustomerName,
                   p.amount AS Amount,
                   r.rental_date AS RentalDate,
                   cat.name AS Category
            FROM rental r
            INNER JOIN customer c ON c.customer_id = r.customer_id
            INNER JOIN payment p ON p.rental_id = r.rental_id
            INNER JOIN inventory i ON i.inventory_id = r.inventory_id
            INNER JOIN film f ON f.film_id = i.film_id
            INNER JOIN film_category fc ON fc.film_id = f.film_id
            INNER JOIN category cat ON cat.category_id = fc.category_id
            WHERE r.rental_date > @Since AND p.amount > 2.00
            ORDER BY r.rental_date
            LIMIT @BatchSize
        ";

        var result = await connection.QueryAsync<CustomerOrderFlat>(sql, new { Since = since, BatchSize });
        return result.ToList();
    }

    public async Task<DateTime> GetLastProcessedTimestampAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        var result = await connection.ExecuteScalarAsync<DateTime?>(
            "SELECT MAX(LastProcessedAt) FROM EtlState"
        );
        return result ?? DateTime.MinValue;
    }

    public async Task UpdateLastProcessedTimestampAsync(DateTime timestamp)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync("INSERT INTO EtlState (LastProcessedAt) VALUES (@Timestamp)", new { Timestamp = timestamp });
    }
}
