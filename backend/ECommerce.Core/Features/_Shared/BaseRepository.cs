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
    /// Execute LINQ query asynchronously and return list
    /// </summary>
    protected async Task<List<T>> ExecuteQueryAsync<T>(IQueryable<T> query) where T : EntityBase2
    {
        // Wrap the LINQ query in Task.Run for async execution
        return await Task.Run(() => query.ToList());
    }

    /// <summary>
    /// Execute LINQ query asynchronously and return single entity
    /// </summary>
    protected async Task<T?> ExecuteQuerySingleAsync<T>(IQueryable<T> query) where T : EntityBase2
    {
        return await Task.Run(() => query.FirstOrDefault());
    }

    /// <summary>
    /// Execute LINQ query count asynchronously
    /// </summary>
    protected async Task<int> ExecuteQueryCountAsync<T>(IQueryable<T> query)
    {
        return await Task.Run(() => query.Count());
    }

    /// <summary>
    /// Execute LINQ query any asynchronously
    /// </summary>
    protected async Task<bool> ExecuteQueryAnyAsync<T>(IQueryable<T> query)
    {
        return await Task.Run(() => query.Any());
    }
}
