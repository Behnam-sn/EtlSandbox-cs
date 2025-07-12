using System.Data;

using EtlSandbox.Domain.EtlApplicationStates.Enums;
using EtlSandbox.Domain.EtlApplicationStates.Repositories;
using EtlSandbox.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.EtlApplicationStates.Repositories;

public sealed class EtlApplicationStateEfCommandRepository : IEtlApplicationStateCommandRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public EtlApplicationStateEfCommandRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<int> GetLastProcessedIdAsync<T>(ProcessType processType)
    {
        return await _applicationDbContext
            .EtlApplicationStates
            .AsNoTracking()
            .Where(item => item.EntityType == typeof(T).Name && item.ProcessType == processType)
            .MaxAsync(item => (int?)item.LastProcessedId) ?? 0;
    }

    public async Task UpdateLastProcessedIdAsync<T>(ProcessType processType, int lastProcessedId, IDbTransaction? transaction = null)
    {
        if (transaction is not null)
        {
            throw new NotSupportedException("Transactional update is not supported for EF-based repository.");
        }

        var entityType = typeof(T).Name;
        var item = await _applicationDbContext
            .EtlApplicationStates
            .SingleOrDefaultAsync(item => item.EntityType == entityType && item.ProcessType == processType);

        if (item is null)
        {
            item = new()
            {
                EntityType = entityType,
                ProcessType = processType,
                LastProcessedId = lastProcessedId
            };
            _applicationDbContext.Add(item);
        }
        else
        {
            item.LastProcessedId = lastProcessedId;
        }

        await _applicationDbContext.SaveChangesAsync();
    }
}