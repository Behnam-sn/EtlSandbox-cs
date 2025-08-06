using EtlSandbox.Application.Common.Abstractions.Messaging;

namespace EtlSandbox.Application.ClickHouseUtils;

public sealed record GetCreateTableQuery(string TableName) : IQuery<string>;