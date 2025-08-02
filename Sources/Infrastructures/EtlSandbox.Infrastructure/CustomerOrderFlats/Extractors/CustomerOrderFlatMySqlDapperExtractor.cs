using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Extractors;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Extractors;

public sealed class CustomerOrderFlatMySqlDapperExtractor(IDbConnectionFactory dbConnectionFactory)
    : BaseDapperExtractor<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string Sql => """
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
                                     WHERE @From < r.rental_id 
                                            AND r.rental_id <= @To
                                     ORDER BY r.rental_id
                                     """;
}