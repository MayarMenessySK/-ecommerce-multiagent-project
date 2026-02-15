namespace ECommerce.Core.Features.Cart;

public interface ICartRepository
{
    Task<CartEntity?> GetByIdAsync(Guid id);
    Task<CartEntity?> GetByUserIdAsync(Guid userId);
    Task<List<CartItemEntity>> GetCartItemsAsync(Guid cartId);
    Task<CartEntity> CreateAsync(CartEntity cart);
    Task<CartEntity> UpdateAsync(CartEntity cart);
    Task<CartItemEntity> AddItemAsync(CartItemEntity item);
    Task<bool> RemoveItemAsync(Guid itemId);
    Task<bool> ClearCartAsync(Guid cartId);
}
