namespace ECommerce.Core.Features.Order;

public class OrderRepository : BaseRepository, IOrderRepository
{
    public OrderRepository(DataAccessAdapter adapter) : base(adapter)
    {
    }

    public async Task<OrderEntity?> GetByIdAsync(Guid id)
    {
        var query = _meta.Order.Where(o => o.Id == id);
        return await ExecuteQuerySingleAsync(query);
    }

    public async Task<List<OrderEntity>> GetByUserIdAsync(Guid userId)
    {
        var query = _meta.Order
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt);
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<List<OrderItemEntity>> GetOrderItemsAsync(Guid orderId)
    {
        var query = _meta.OrderItem
            .Where(oi => oi.OrderId == orderId);
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<OrderEntity> CreateAsync(OrderEntity order)
    {
        order.Id = Guid.NewGuid();
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;
        
        await SaveAsync(order);
        return order;
    }

    public async Task<OrderEntity> UpdateAsync(OrderEntity order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        await SaveAsync(order);
        return order;
    }

    public async Task<OrderItemEntity> AddItemAsync(OrderItemEntity item)
    {
        item.Id = Guid.NewGuid();
        item.CreatedAt = DateTime.UtcNow;
        
        await SaveAsync(item);
        return item;
    }

    public async Task<bool> UpdateStatusAsync(Guid orderId, string status)
    {
        var order = await GetByIdAsync(orderId);
        if (order == null) return false;
        
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        return await SaveAsync(order);
    }
}
