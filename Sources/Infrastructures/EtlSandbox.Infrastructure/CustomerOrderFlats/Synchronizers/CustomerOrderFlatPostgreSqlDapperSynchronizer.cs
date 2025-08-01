﻿using EtlSandbox.Domain.CustomerOrderFlats.Entities;
using EtlSandbox.Domain.Shared;
using EtlSandbox.Infrastructure.Shared.Synchronizers;

namespace EtlSandbox.Infrastructure.CustomerOrderFlats.Synchronizers;

public sealed class CustomerOrderFlatPostgreSqlDapperSynchronizer(IDbConnectionFactory dbConnectionFactory)
    : BaseDapperSynchronizer<CustomerOrderFlat>(dbConnectionFactory)
{
    protected override string Sql => """
                                     UPDATE "CustomerOrderFlats" T
                                     SET "IsDeleted" = '1'
                                     FROM (
                                         SELECT "CustomerName", MAX("Id") AS "MaxId"
                                         FROM "CustomerOrderFlats"
                                         WHERE "Id" BETWEEN @FromId AND @ToId
                                         GROUP BY "CustomerName"
                                     ) Latest
                                     WHERE T."CustomerName" = Latest."CustomerName"
                                     AND T."Id" < Latest."MaxId"
                                     AND T."IsDeleted" = '0'
                                     """;
}