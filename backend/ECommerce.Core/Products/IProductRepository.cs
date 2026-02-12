using ECommerce.Core.Misc;
using ECommerce.Core.Models;

namespace ECommerce.Core.Products;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<Product?> GetBySlugAsync(string slug);
    Task<PaginatedResult<Product>> GetAllAsync(ProductFilter filter);
    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<List<ProductImage>> GetProductImagesAsync(Guid productId);
    Task<ProductImage> AddImageAsync(ProductImage image);
    Task DeleteImageAsync(Guid imageId);
    Task UpdateStockAsync(Guid productId, int quantity);
    Task UpdateRatingAsync(Guid productId);
}

public class ProductFilter : PaginationFilter
{
    public Guid? CategoryId { get; set; }
    public string? Search { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Brand { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? InStock { get; set; }
}
