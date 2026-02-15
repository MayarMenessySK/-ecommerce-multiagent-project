namespace ECommerce.Core.Features.Cart;

public class CartRepository : BaseRepository, ICartRepository
{
    public CartRepository(DataAccessAdapter adapter) : base(adapter)
    {
    }

    public async Task<CartEntity?> GetByIdAsync(Guid id)
    {
        return await GetByIdAsync<CartEntity>(id);
    }

    public async Task<CartEntity?> GetByUserIdAsync(Guid userId)
    {
        return await _meta.Cart
            .Where(c => c.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<CartItemEntity>> GetCartItemsAsync(Guid cartId)
    {
        return await _meta.CartItem
            .Where(ci => ci.CartId == cartId)
            .ToListAsync();
    }

    public async Task<CartEntity> CreateAsync(CartEntity cart)
    {
        cart.Id = Guid.NewGuid();
        cart.CreatedAt = DateTime.UtcNow;
        cart.UpdatedAt = DateTime.UtcNow;
        
        await SaveAsync(cart);
        return cart;
    }

    public async Task<CartEntity> UpdateAsync(CartEntity cart)
    {
        cart.UpdatedAt = DateTime.UtcNow;
        await SaveAsync(cart);
        return cart;
    }

    public async Task<CartItemEntity> AddItemAsync(CartItemEntity item)
    {
        item.Id = Guid.NewGuid();
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        
        await SaveAsync(item);
        return item;
    }

    public async Task<bool> RemoveItemAsync(Guid itemId)
    {
        var item = await GetByIdAsync<CartItemEntity>(itemId);
        if (item == null) return false;
        
        return await DeleteAsync(item);
    }

    public async Task<bool> ClearCartAsync(Guid cartId)
    {
        var items = await GetCartItemsAsync(cartId);
        
        foreach (var item in items)
        {
            await DeleteAsync(item);
        }
        
        return true;
    }
}
