using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Categories;

public class CreateCategoryInput
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Url]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public Guid? ParentCategoryId { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateCategoryInput
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Url]
    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public Guid? ParentCategoryId { get; set; }

    [Range(0, int.MaxValue)]
    public int DisplayOrder { get; set; } = 0;
}
