namespace ECommerce.Core.Features.Product;

/// <summary>
/// Product repository interface using LLBLGen entities
/// </summary>
public interface IProductRepository
{
    Task<ProductEntity?> GetByIdAsync(Guid id);
    Task<ProductEntity?> GetBySlugAsync(string slug);
    Task<List<ProductEntity>> GetAllAsync();
    Task<List<ProductEntity>> GetByCategoryAsync(Guid categoryId);
    Task<List<ProductEntity>> GetFeaturedAsync();
    Task<List<ProductEntity>> SearchAsync(string searchTerm);
    Task<ProductEntity> CreateAsync(ProductEntity product);
    Task<ProductEntity> UpdateAsync(ProductEntity product);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> UpdateStockAsync(Guid id, int quantity);
}
