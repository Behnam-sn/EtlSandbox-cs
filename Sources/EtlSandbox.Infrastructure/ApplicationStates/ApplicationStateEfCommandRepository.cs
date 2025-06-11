using EtlSandbox.Domain.ApplicationStates;
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

    public async Task<int> GetLastProcessedIdAsync<T>(ActionType actionType)
    {
        return await _applicationDbContext
            .ApplicationStates
            .AsNoTracking()
            .Where(item => item.EntityType == typeof(T).Name && item.ActionType == actionType)
            .MaxAsync(item => (int?)item.LastProcessedId) ?? 0;
    }

    public async Task UpdateLastProcessedIdAsync<T>(ActionType actionType, int lastProcessedId)
    {
        var entityType = typeof(T).Name;
        var item = await _applicationDbContext
            .ApplicationStates
            .SingleOrDefaultAsync(item => item.EntityType == entityType && item.ActionType == actionType);

        if (item is null)
        {
            item = new()
            {
                EntityType = entityType,
                ActionType = actionType,
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
