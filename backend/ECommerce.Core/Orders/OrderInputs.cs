using System.ComponentModel.DataAnnotations;

namespace ECommerce.Core.Orders;

public class CreateOrderInput
{
    [Required]
    [StringLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string ShippingMethod { get; set; } = string.Empty;

    [Required]
    public ShippingAddressInput ShippingAddress { get; set; } = new();

    public BillingAddressInput? BillingAddress { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }

    [StringLength(50)]
    public string? DiscountCode { get; set; }
}

public class ShippingAddressInput
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(255)]
    public string? AddressLine2 { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;
}

public class BillingAddressInput
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string AddressLine1 { get; set; } = string.Empty;

    [StringLength(255)]
    public string? AddressLine2 { get; set; }

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Country { get; set; } = string.Empty;
}

public class UpdateOrderStatusInput
{
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;
}

public class CancelOrderInput
{
    [Required]
    [StringLength(500)]
    public string CancellationReason { get; set; } = string.Empty;
}
