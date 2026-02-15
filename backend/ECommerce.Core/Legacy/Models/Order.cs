namespace ECommerce.Core.Models;

public class Order
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; } = 0;
    public decimal ShippingCost { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public decimal Total { get; set; }
    public string Currency { get; set; } = "USD";
    public string? DiscountCode { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = "Pending";
    public string? PaymentTransactionId { get; set; }
    public string ShippingMethod { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
    public string? ShippingCarrier { get; set; }
    public string ShippingFullName { get; set; } = string.Empty;
    public string ShippingPhone { get; set; } = string.Empty;
    public string ShippingAddressLine1 { get; set; } = string.Empty;
    public string? ShippingAddressLine2 { get; set; }
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingState { get; set; } = string.Empty;
    public string ShippingPostalCode { get; set; } = string.Empty;
    public string ShippingCountry { get; set; } = string.Empty;
    public string? BillingFullName { get; set; }
    public string? BillingPhone { get; set; }
    public string? BillingAddressLine1 { get; set; }
    public string? BillingAddressLine2 { get; set; }
    public string? BillingCity { get; set; }
    public string? BillingState { get; set; }
    public string? BillingPostalCode { get; set; }
    public string? BillingCountry { get; set; }
    public string? Notes { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<OrderItem> Items { get; set; } = new();
}
