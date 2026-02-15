namespace ECommerce.Core.Products;

public class ProductOutput
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
    public int StockQuantity { get; set; }
    public bool IsInStock { get; set; }
    public bool IsLowStock { get; set; }
    public Guid CategoryId { get; set; }
    public string? Brand { get; set; }
    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalSales { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ProductImageOutput> Images { get; set; } = new();
}

public class ProductImageOutput
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
}

public class ProductListOutput
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public bool IsInStock { get; set; }
    public string? Brand { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public string? PrimaryImage { get; set; }
}
