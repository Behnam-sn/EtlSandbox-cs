using EtlSandbox.Domain.Common.DbConnectionFactories;
using EtlSandbox.Domain.Common.Options;
using EtlSandbox.Infrastructure.Common.DbConnectionFactories.Bases;

using Microsoft.Extensions.Options;

namespace EtlSandbox.Infrastructure.Common.DbConnectionFactories.Destinations;

public sealed class DestinationClickHouseConnectionFactory(IOptions<ConnectionStrings> options)
    : BaseClickHouseConnectionFactory(options.Value.Destination), IDestinationDbConnectionFactory;