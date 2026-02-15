using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Products;

public class CreateProductInput
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Sku { get; set; } = string.Empty;

    [StringLength(5000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? ShortDescription { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [StringLength(100)]
    public string? Brand { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
}

public class UpdateProductInput
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(5000)]
    public string? Description { get; set; }

    [StringLength(500)]
    public string? ShortDescription { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    public decimal? OriginalPrice { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [StringLength(100)]
    public string? Brand { get; set; }

    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }
    public bool IsFeatured { get; set; }
}

public class AddProductImageInput
{
    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Url]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(255)]
    public string? AltText { get; set; }

    public bool IsPrimary { get; set; } = false;
}
