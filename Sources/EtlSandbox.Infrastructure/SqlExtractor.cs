using Dapper;

using EtlSandbox.Domain;
using EtlSandbox.Domain.Configurations;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

namespace EtlSandbox.Infrastructure;

public sealed class SqlExtractor : IExtractor<CustomerOrderFlat>
{
    private const int BatchSize = 100_000;
    private readonly ILogger<SqlExtractor> _logger;
    private readonly string _sourceConnectionString;
    private readonly string _destinationConnectionString;
    public SqlExtractor(ILogger<SqlExtractor> logger, IOptions<ConnectionStrings> options)
    {
        _logger = logger;
        _sourceConnectionString = options.Value.MySql;
        _destinationConnectionString = options.Value.SqlServer;
    }

    public async Task<IReadOnlyList<CustomerOrderFlat>> ExtractAsync(DateTime since, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Extracting data since {Since}", since);
        using var connection = new MySqlConnection(_sourceConnectionString);
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
        _logger.LogInformation("Extracted {Count} rows", result.Count());
        return result.ToList();
    }

    public async Task<DateTime> GetLastProcessedTimestampAsync()
    {
        using var connection = new SqlConnection(_destinationConnectionString);
        var result = await connection.ExecuteScalarAsync<DateTime?>(
            "SELECT MAX(LastProcessedAt) FROM EtlStates"
        );
        return result ?? DateTime.MinValue;
    }

    public async Task UpdateLastProcessedTimestampAsync(DateTime timestamp)
    {
        using var connection = new SqlConnection(_destinationConnectionString);
        await connection.ExecuteAsync("INSERT INTO EtlStates (LastProcessedAt) VALUES (@Timestamp)", new { Timestamp = timestamp });

        _logger.LogInformation("Updated last processed timestamp to {Timestamp}", timestamp);
    }
}
