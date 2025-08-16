using EtlSandbox.Domain.Common.DbConnectionFactories;
using EtlSandbox.Domain.Common.Options;
using EtlSandbox.Infrastructure.Common.DbConnectionFactories.Bases;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories.Sources;

public sealed class SourceSqlServerConnectionFactory(IOptions<ConnectionStrings> options)
    : BaseSqlServerConnectionFactory(options.Value.Source), ISourceDbConnectionFactory;