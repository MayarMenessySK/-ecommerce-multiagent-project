using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Products;

public class UpdateProductStatusInput
{
    [Required]
    public bool IsActive { get; set; }
}
