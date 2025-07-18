﻿using Dapper;

using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Domain.Shared.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatMySqlDapperExtractor : IExtractor<CustomerOrderFlat>
{
    private readonly ILogger<CustomerOrderFlatMySqlDapperExtractor> _logger;

    private readonly string _sourceConnectionString;

    public CustomerOrderFlatMySqlDapperExtractor(ILogger<CustomerOrderFlatMySqlDapperExtractor> logger, IOptions<DatabaseConnections> options)
    {
        _logger = logger;
        _sourceConnectionString = options.Value.Source;
    }

    public async Task<List<CustomerOrderFlat>> ExtractAsync(int lastProcessedId, int batchSize, CancellationToken cancellationToken)
    {
        await using var connection = new MySqlConnection(_sourceConnectionString);
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
            WHERE r.rental_id > @LastProcessedId
            ORDER BY r.rental_id
            LIMIT @BatchSize
        ";

        var result = await connection.QueryAsync<CustomerOrderFlat>(sql, new
        {
            LastProcessedId = lastProcessedId,
            BatchSize = batchSize
        });

        var items = result.ToList();
        return items;
    }
}