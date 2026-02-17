namespace ECommerce.Core.Features.Product;

/// <summary>
/// Product repository implementation using LLBLGen Pro
/// </summary>
public class ProductRepository : BaseRepository, IProductRepository
{
    public ProductRepository(DataAccessAdapter adapter) : base(adapter)
    {
    }

    public async Task<ProductEntity?> GetByIdAsync(Guid id)
    {
        var query = _meta.Product.Where(p => p.Id == id);
        return await ExecuteQuerySingleAsync(query);
    }

    public async Task<ProductEntity?> GetBySlugAsync(string slug)
    {
        var query = _meta.Product.Where(p => p.Slug == slug);
        return await ExecuteQuerySingleAsync(query);
    }

    public async Task<List<ProductEntity>> GetAllAsync()
    {
        var query = _meta.Product
            .OrderByDescending(p => p.CreatedAt);
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<List<ProductEntity>> GetByCategoryAsync(Guid categoryId)
    {
        var query = _meta.Product
            .Where(p => p.CategoryId == categoryId && p.IsActive)
            .OrderByDescending(p => p.CreatedAt);
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<List<ProductEntity>> GetFeaturedAsync()
    {
        var query = _meta.Product
            .Where(p => p.IsFeatured && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .Take(10);
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<List<ProductEntity>> SearchAsync(string searchTerm)
    {
        var lowerSearch = searchTerm.ToLower();
        
        var query = _meta.Product
            .Where(p => p.IsActive && 
                   (p.Name.ToLower().Contains(lowerSearch) ||
                    (p.Description != null && p.Description.ToLower().Contains(lowerSearch)) ||
                    p.Sku.ToLower().Contains(lowerSearch)))
            .OrderByDescending(p => p.CreatedAt);
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<ProductEntity> CreateAsync(ProductEntity product)
    {
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;
        
        await SaveAsync(product);
        return product;
    }

    public async Task<ProductEntity> UpdateAsync(ProductEntity product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        await SaveAsync(product);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await GetByIdAsync(id);
        if (product == null) return false;
        
        // Soft delete
        product.IsActive = false;
        product.UpdatedAt = DateTime.UtcNow;
        return await SaveAsync(product);
    }

    public async Task<bool> UpdateStockAsync(Guid id, int quantity)
    {
        var product = await GetByIdAsync(id);
        if (product == null) return false;
        
        product.StockQuantity = quantity;
        product.UpdatedAt = DateTime.UtcNow;
        return await SaveAsync(product);
    }
}
