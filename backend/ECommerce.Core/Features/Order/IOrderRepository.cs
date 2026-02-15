namespace ECommerce.Core.Features.Order;

public interface IOrderRepository
{
    Task<OrderEntity?> GetByIdAsync(Guid id);
    Task<List<OrderEntity>> GetByUserIdAsync(Guid userId);
    Task<List<OrderItemEntity>> GetOrderItemsAsync(Guid orderId);
    Task<OrderEntity> CreateAsync(OrderEntity order);
    Task<OrderEntity> UpdateAsync(OrderEntity order);
    Task<OrderItemEntity> AddItemAsync(OrderItemEntity item);
    Task<bool> UpdateStatusAsync(Guid orderId, string status);
}
