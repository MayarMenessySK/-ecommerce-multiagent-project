using SD.LLBLGen.Pro.ORMSupportClasses;
using ECommerce.Data.EntityClasses;
using ECommerce.Data.DatabaseSpecific;

namespace ECommerce.Data.Repositories;

/// <summary>
/// Base repository providing common CRUD operations using LLBLGen Pro
/// </summary>
/// <typeparam name="TEntity">The entity type (must inherit from EntityBase2)</typeparam>
public abstract class BaseRepository<TEntity> where TEntity : EntityBase2, new()
{
    protected readonly string _connectionString;

    protected BaseRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Get entity by ID
    /// </summary>
    public virtual async Task<TEntity?> GetByIdAsync(Guid id, PrefetchPath2? prefetchPath = null)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        var entity = (TEntity)Activator.CreateInstance(typeof(TEntity), id)!;
        
        var fetched = await adapter.FetchEntityAsync(entity, prefetchPath);
        return fetched && !entity.IsNew ? entity : null;
    }

    /// <summary>
    /// Get all entities with optional filtering and sorting
    /// </summary>
    public virtual async Task<List<TEntity>> GetAllAsync(
        IRelationPredicateBucket? filter = null, 
        ISortExpression? sorter = null,
        PrefetchPath2? prefetchPath = null)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        var entities = new EntityCollection<TEntity>();
        
        await adapter.FetchEntityCollectionAsync(entities, filter, 0, sorter, prefetchPath);
        return entities.ToList();
    }

    /// <summary>
    /// Get paged entities
    /// </summary>
    public virtual async Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        IRelationPredicateBucket? filter = null,
        ISortExpression? sorter = null,
        PrefetchPath2? prefetchPath = null)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        var entities = new EntityCollection<TEntity>();
        
        // Get total count
        var totalCount = await adapter.GetDbCountAsync(filter, entities);
        
        // Get paged data
        await adapter.FetchEntityCollectionAsync(
            entities, 
            filter, 
            pageSize, 
            sorter, 
            prefetchPath, 
            pageNumber, 
            pageSize);
        
        return (entities.ToList(), (int)totalCount);
    }

    /// <summary>
    /// Save entity (insert or update)
    /// </summary>
    public virtual async Task<bool> SaveAsync(TEntity entity, bool refetchAfterSave = true)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        return await adapter.SaveEntityAsync(entity, refetchAfterSave, true);
    }

    /// <summary>
    /// Save multiple entities in a transaction
    /// </summary>
    public virtual async Task<int> SaveCollectionAsync(EntityCollection<TEntity> entities)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        return await adapter.SaveEntityCollectionAsync(entities, refetchSavedEntitiesAfterSave: false, recurse: true);
    }

    /// <summary>
    /// Delete entity by ID
    /// </summary>
    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        var entity = (TEntity)Activator.CreateInstance(typeof(TEntity), id)!;
        return await adapter.DeleteEntityAsync(entity);
    }

    /// <summary>
    /// Delete entities matching filter
    /// </summary>
    public virtual async Task<int> DeleteMultipleAsync(IRelationPredicateBucket filter)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        var entities = new EntityCollection<TEntity>();
        return await adapter.DeleteEntitiesDirectlyAsync(typeof(TEntity).Name, filter);
    }

    /// <summary>
    /// Check if entity exists
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        var entity = (TEntity)Activator.CreateInstance(typeof(TEntity), id)!;
        return await adapter.FetchEntityAsync(entity);
    }

    /// <summary>
    /// Count entities matching filter
    /// </summary>
    public virtual async Task<int> CountAsync(IRelationPredicateBucket? filter = null)
    {
        using var adapter = new DataAccessAdapter(_connectionString);
        var entities = new EntityCollection<TEntity>();
        return (int)await adapter.GetDbCountAsync(filter, entities);
    }
}
