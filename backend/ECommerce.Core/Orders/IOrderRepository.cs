using ECommerce.Core.Misc;
using ECommerce.Core.Models;

namespace ECommerce.Core.Orders;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<PaginatedResult<Order>> GetUserOrdersAsync(Guid userId, PaginationFilter filter);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<Order> CancelAsync(Guid orderId, string cancellationReason);
    Task<List<OrderItem>> GetOrderItemsAsync(Guid orderId);
    Task<OrderItem> AddOrderItemAsync(OrderItem item);
    Task UpdateOrderStatusAsync(Guid orderId, string status);
}
