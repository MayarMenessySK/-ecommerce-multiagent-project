using ECommerce.Core.Models;

namespace ECommerce.Core.Cart;

public interface ICartRepository
{
    Task<Models.Cart?> GetByIdAsync(Guid id);
    Task<Models.Cart?> GetByUserIdAsync(Guid userId);
    Task<Models.Cart?> GetBySessionIdAsync(string sessionId);
    Task<Models.Cart> GetOrCreateForUserAsync(Guid userId);
    Task<Models.Cart> GetOrCreateForSessionAsync(string sessionId);
    Task<Models.Cart> CreateAsync(Models.Cart cart);
    Task<Models.Cart> UpdateAsync(Models.Cart cart);
    Task DeleteAsync(Guid id);
    Task<List<CartItem>> GetCartItemsAsync(Guid cartId);
    Task<CartItem> AddItemAsync(CartItem item);
    Task<CartItem> UpdateItemAsync(CartItem item);
    Task RemoveItemAsync(Guid itemId);
    Task ClearCartAsync(Guid cartId);
    Task RecalculateTotalsAsync(Guid cartId);
}
