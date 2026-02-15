namespace ECommerce.Core.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public string Currency { get; set; } = "USD";
    public int StockQuantity { get; set; } = 0;
    public int LowStockThreshold { get; set; } = 10;
    public Guid CategoryId { get; set; }
    public string? Brand { get; set; }
    public decimal? Weight { get; set; }
    public string? Dimensions { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public int TotalSales { get; set; } = 0;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<ProductImage> Images { get; set; } = new();
    public bool IsInStock => StockQuantity > 0;
    public bool IsLowStock => StockQuantity > 0 && StockQuantity <= LowStockThreshold;
}
