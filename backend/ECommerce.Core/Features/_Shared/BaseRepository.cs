namespace ECommerce.Core.Features._Shared;

/// <summary>
/// Base repository using LLBLGen Pro for data access
/// All feature repositories inherit from this
/// </summary>
public abstract class BaseRepository
{
    protected readonly DataAccessAdapter _adapter;
    protected readonly LinqMetaData _meta;

    protected BaseRepository(DataAccessAdapter adapter)
    {
        _adapter = adapter;
        _meta = new LinqMetaData(_adapter);
    }

    /// <summary>
    /// Get entity by ID using LLBLGen
    /// </summary>
    protected async Task<T?> GetByIdAsync<T>(Guid id) where T : EntityBase2, new()
    {
        var entity = new T();
        var success = await _adapter.FetchEntityAsync(entity, id);
        return success ? entity : null;
    }

    /// <summary>
    /// Get all entities of a type
    /// </summary>
    protected async Task<List<T>> GetAllAsync<T>() where T : EntityBase2, new()
    {
        var collection = new EntityCollection<T>();
        await _adapter.FetchEntityCollectionAsync(collection, null);
        return collection.ToList();
    }

    /// <summary>
    /// Save entity (insert or update)
    /// </summary>
    protected async Task<bool> SaveAsync<T>(T entity) where T : EntityBase2
    {
        return await _adapter.SaveEntityAsync(entity);
    }

    /// <summary>
    /// Delete entity
    /// </summary>
    protected async Task<bool> DeleteAsync<T>(T entity) where T : EntityBase2
    {
        return await _adapter.DeleteEntityAsync(entity);
    }

    /// <summary>
    /// Execute query and return entities
    /// </summary>
    protected async Task<List<T>> FetchAsync<T>(IQuery<T> query) where T : EntityBase2
    {
        var collection = new EntityCollection<T>();
        await _adapter.FetchEntityCollectionAsync(collection, query);
        return collection.ToList();
    }
}
