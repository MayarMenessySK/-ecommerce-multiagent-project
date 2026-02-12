using AutoMapper;
using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using ECommerce.Core.Products;

namespace ECommerce.Core.Cart;

public class CartService : BaseService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<CartOutput> GetCartAsync(Guid? userId = null, string? sessionId = null)
    {
        Models.Cart? cart;

        if (userId.HasValue)
        {
            cart = await _cartRepository.GetOrCreateForUserAsync(userId.Value);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            cart = await _cartRepository.GetOrCreateForSessionAsync(sessionId);
        }
        else
        {
            throw new ArgumentException("Either userId or sessionId must be provided");
        }

        return _mapper.Map<CartOutput>(cart);
    }

    public async Task<CartOutput> AddToCartAsync(AddToCartInput input, Guid? userId = null, string? sessionId = null)
    {
        ValidateEntity(input);

        // Validate product exists and is in stock
        var product = await _productRepository.GetByIdAsync(input.ProductId);
        if (product == null)
        {
            ThrowNotFound("Product", input.ProductId);
        }

        if (!product!.IsActive)
        {
            ThrowBadRequest("Product is not available");
        }

        if (product.StockQuantity < input.Quantity)
        {
            ThrowBadRequest($"Insufficient stock. Only {product.StockQuantity} units available");
        }

        // Get or create cart
        Models.Cart cart;
        if (userId.HasValue)
        {
            cart = await _cartRepository.GetOrCreateForUserAsync(userId.Value);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            cart = await _cartRepository.GetOrCreateForSessionAsync(sessionId);
        }
        else
        {
            throw new ArgumentException("Either userId or sessionId must be provided");
        }

        // Check if item already exists in cart
        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == input.ProductId);
        if (existingItem != null)
        {
            // Update existing item
            var newQuantity = existingItem.Quantity + input.Quantity;
            if (product.StockQuantity < newQuantity)
            {
                ThrowBadRequest($"Insufficient stock. Only {product.StockQuantity} units available");
            }

            existingItem.Quantity = newQuantity;
            existingItem.Subtotal = existingItem.Price * newQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
            await _cartRepository.UpdateItemAsync(existingItem);
        }
        else
        {
            // Add new item
            var newItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = input.ProductId,
                Quantity = input.Quantity,
                Price = product.Price,
                Subtotal = product.Price * input.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _cartRepository.AddItemAsync(newItem);
        }

        // Recalculate cart totals
        await _cartRepository.RecalculateTotalsAsync(cart.Id);

        // Fetch updated cart
        var updatedCart = await _cartRepository.GetByIdAsync(cart.Id);
        return _mapper.Map<CartOutput>(updatedCart);
    }

    public async Task<CartOutput> UpdateCartItemAsync(Guid itemId, UpdateCartItemInput input, Guid? userId = null, string? sessionId = null)
    {
        ValidateId(itemId);
        ValidateEntity(input);

        // Get cart to verify ownership
        Models.Cart? cart;
        if (userId.HasValue)
        {
            cart = await _cartRepository.GetByUserIdAsync(userId.Value);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            cart = await _cartRepository.GetBySessionIdAsync(sessionId);
        }
        else
        {
            throw new ArgumentException("Either userId or sessionId must be provided");
        }

        if (cart == null)
        {
            ThrowNotFound("Cart not found");
        }

        var item = cart!.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            ThrowNotFound("Cart item", itemId);
        }

        // Validate stock
        var product = await _productRepository.GetByIdAsync(item!.ProductId);
        if (product == null)
        {
            ThrowNotFound("Product", item.ProductId);
        }

        if (product!.StockQuantity < input.Quantity)
        {
            ThrowBadRequest($"Insufficient stock. Only {product.StockQuantity} units available");
        }

        // Update item
        item.Quantity = input.Quantity;
        item.Subtotal = item.Price * input.Quantity;
        item.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.UpdateItemAsync(item);

        // Recalculate cart totals
        await _cartRepository.RecalculateTotalsAsync(cart.Id);

        // Fetch updated cart
        var updatedCart = await _cartRepository.GetByIdAsync(cart.Id);
        return _mapper.Map<CartOutput>(updatedCart);
    }

    public async Task<CartOutput> RemoveCartItemAsync(Guid itemId, Guid? userId = null, string? sessionId = null)
    {
        ValidateId(itemId);

        // Get cart to verify ownership
        Models.Cart? cart;
        if (userId.HasValue)
        {
            cart = await _cartRepository.GetByUserIdAsync(userId.Value);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            cart = await _cartRepository.GetBySessionIdAsync(sessionId);
        }
        else
        {
            throw new ArgumentException("Either userId or sessionId must be provided");
        }

        if (cart == null)
        {
            ThrowNotFound("Cart not found");
        }

        var item = cart!.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            ThrowNotFound("Cart item", itemId);
        }

        // Remove item
        await _cartRepository.RemoveItemAsync(itemId);

        // Recalculate cart totals
        await _cartRepository.RecalculateTotalsAsync(cart.Id);

        // Fetch updated cart
        var updatedCart = await _cartRepository.GetByIdAsync(cart.Id);
        return _mapper.Map<CartOutput>(updatedCart);
    }

    public async Task<CartOutput> ClearCartAsync(Guid? userId = null, string? sessionId = null)
    {
        // Get cart
        Models.Cart? cart;
        if (userId.HasValue)
        {
            cart = await _cartRepository.GetByUserIdAsync(userId.Value);
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            cart = await _cartRepository.GetBySessionIdAsync(sessionId);
        }
        else
        {
            throw new ArgumentException("Either userId or sessionId must be provided");
        }

        if (cart == null)
        {
            ThrowNotFound("Cart not found");
        }

        // Clear all items
        await _cartRepository.ClearCartAsync(cart!.Id);

        // Recalculate cart totals (will be zero)
        await _cartRepository.RecalculateTotalsAsync(cart.Id);

        // Fetch updated cart
        var updatedCart = await _cartRepository.GetByIdAsync(cart.Id);
        return _mapper.Map<CartOutput>(updatedCart);
    }
}
