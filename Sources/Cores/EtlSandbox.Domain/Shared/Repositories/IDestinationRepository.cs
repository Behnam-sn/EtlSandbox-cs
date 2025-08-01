﻿namespace EtlSandbox.Domain.Shared.Repositories;

public interface IDestinationRepository<T>
    where T : class, IEntity
{
    Task<long> GetLastInsertedImportantIdAsync();

    Task<long> GetLastSoftDeletedItemIdAsync();

    Task<long> GetLastItemIdAsync();
}