namespace ECommerce.Core.Models;

public class Cart
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string? SessionId { get; set; }
    public decimal Subtotal { get; set; } = 0;
    public decimal Tax { get; set; } = 0;
    public decimal Shipping { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public decimal Total { get; set; } = 0;
    public string? DiscountCode { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<CartItem> Items { get; set; } = new();
}
