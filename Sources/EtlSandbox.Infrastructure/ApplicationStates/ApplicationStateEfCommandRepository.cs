using System.Data;

using EtlSandbox.Domain.ApplicationStates;
using EtlSandbox.Domain.ApplicationStates.Enums;
using EtlSandbox.Domain.ApplicationStates.Repositories;
using EtlSandbox.Infrastructure.DbContexts;

using Microsoft.EntityFrameworkCore;

namespace EtlSandbox.Infrastructure.ApplicationStates;

public sealed class ApplicationStateEfCommandRepository : IApplicationStateCommandRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public ApplicationStateEfCommandRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<int> GetLastProcessedIdAsync<T>(ProcessType processType)
    {
        return await _applicationDbContext
            .ApplicationStates
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
            .ApplicationStates
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