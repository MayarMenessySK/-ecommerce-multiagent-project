using ECommerce.Data.EntityClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace ECommerce.Data.Repositories;

/// <summary>
/// Repository interface for Product operations
/// This will be implemented once LLBLGen entities are generated
/// </summary>
public interface IProductRepository
{
    // Basic CRUD
    Task<ProductEntity?> GetByIdAsync(Guid id);
    Task<ProductEntity?> GetBySlugAsync(string slug);
    Task<ProductEntity?> GetBySkuAsync(string sku);
    Task<(List<ProductEntity> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, ProductFilter filter);
    Task<List<ProductEntity>> GetFeaturedAsync(int count = 10);
    Task<List<ProductEntity>> GetByCategoryAsync(Guid categoryId, int pageNumber = 1, int pageSize = 20);
    Task<List<ProductEntity>> SearchAsync(string searchTerm, int pageNumber = 1, int pageSize = 20);
    Task<bool> SaveAsync(ProductEntity product);
    Task<bool> DeleteAsync(Guid id);
    
    // Stock management
    Task<bool> UpdateStockAsync(Guid productId, int quantity);
    Task<bool> DecrementStockAsync(Guid productId, int quantity);
    Task<List<ProductEntity>> GetLowStockProductsAsync();
    
    // Rating and reviews
    Task<bool> UpdateRatingAsync(Guid productId, decimal averageRating, int totalReviews);
    
    // Advanced queries
    Task<List<ProductEntity>> GetRelatedProductsAsync(Guid productId, int count = 6);
    Task<List<ProductEntity>> GetNewArrivalsAsync(int days = 30, int count = 10);
    Task<List<ProductEntity>> GetTopSellingAsync(int count = 10);
}

/// <summary>
/// Product filter for search and filtering
/// </summary>
public class ProductFilter
{
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Brand { get; set; }
    public bool? IsActive { get; set; } = true;
    public bool? IsFeatured { get; set; }
    public bool? InStock { get; set; }
    public decimal? MinRating { get; set; }
    public string? SortBy { get; set; } = "created_at"; // created_at, price, name, rating, sales
    public bool SortDescending { get; set; } = true;
}
