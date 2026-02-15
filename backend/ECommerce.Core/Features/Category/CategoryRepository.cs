namespace ECommerce.Core.Features.Category;

public class CategoryRepository : BaseRepository, ICategoryRepository
{
    public CategoryRepository(DataAccessAdapter adapter) : base(adapter)
    {
    }

    public async Task<CategoryEntity?> GetByIdAsync(Guid id)
    {
        return await GetByIdAsync<CategoryEntity>(id);
    }

    public async Task<CategoryEntity?> GetBySlugAsync(string slug)
    {
        return await _meta.Category
            .Where(c => c.Slug == slug)
            .FirstOrDefaultAsync();
    }

    public async Task<List<CategoryEntity>> GetAllAsync()
    {
        return await _meta.Category
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<CategoryEntity>> GetActiveAsync()
    {
        return await _meta.Category
            .Where(c => c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<CategoryEntity> CreateAsync(CategoryEntity category)
    {
        category.Id = Guid.NewGuid();
        category.CreatedAt = DateTime.UtcNow;
        category.UpdatedAt = DateTime.UtcNow;
        
        await SaveAsync(category);
        return category;
    }

    public async Task<CategoryEntity> UpdateAsync(CategoryEntity category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        await SaveAsync(category);
        return category;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await GetByIdAsync(id);
        if (category == null) return false;
        
        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        return await SaveAsync(category);
    }
}
