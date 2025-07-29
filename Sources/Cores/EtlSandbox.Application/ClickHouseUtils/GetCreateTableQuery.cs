using EtlSandbox.Application.Shared.Abstractions.Messaging;

namespace EtlSandbox.Application.ClickHouseUtils;

public sealed record GetCreateTableQuery(string TableName) : IQuery<string>;