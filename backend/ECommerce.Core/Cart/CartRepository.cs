using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using System.Data;
using System.Text;

namespace ECommerce.Core.Cart;

public class CartRepository : BaseRepository, ICartRepository
{
    public CartRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<Models.Cart?> GetByIdAsync(Guid id)
    {
        var sql = @"
            SELECT c.*, 
                   ci.id as item_id, ci.cart_id, ci.product_id, ci.quantity, 
                   ci.price, ci.subtotal, ci.created_at as item_created_at, ci.updated_at as item_updated_at,
                   p.name as product_name, p.slug as product_slug, 
                   pi.image_url as product_image
            FROM carts c
            LEFT JOIN cart_items ci ON c.id = ci.cart_id
            LEFT JOIN products p ON ci.product_id = p.id
            LEFT JOIN (
                SELECT DISTINCT ON (product_id) product_id, image_url
                FROM product_images
                WHERE is_primary = true
                ORDER BY product_id, display_order
            ) pi ON ci.product_id = pi.product_id
            WHERE c.id = @Id
            ORDER BY ci.created_at ASC";

        return await GetCartWithItems(sql, new { Id = id });
    }

    public async Task<Models.Cart?> GetByUserIdAsync(Guid userId)
    {
        var sql = @"
            SELECT c.*, 
                   ci.id as item_id, ci.cart_id, ci.product_id, ci.quantity, 
                   ci.price, ci.subtotal, ci.created_at as item_created_at, ci.updated_at as item_updated_at,
                   p.name as product_name, p.slug as product_slug, 
                   pi.image_url as product_image
            FROM carts c
            LEFT JOIN cart_items ci ON c.id = ci.cart_id
            LEFT JOIN products p ON ci.product_id = p.id
            LEFT JOIN (
                SELECT DISTINCT ON (product_id) product_id, image_url
                FROM product_images
                WHERE is_primary = true
                ORDER BY product_id, display_order
            ) pi ON ci.product_id = pi.product_id
            WHERE c.user_id = @UserId
            ORDER BY ci.created_at ASC";

        return await GetCartWithItems(sql, new { UserId = userId });
    }

    public async Task<Models.Cart?> GetBySessionIdAsync(string sessionId)
    {
        var sql = @"
            SELECT c.*, 
                   ci.id as item_id, ci.cart_id, ci.product_id, ci.quantity, 
                   ci.price, ci.subtotal, ci.created_at as item_created_at, ci.updated_at as item_updated_at,
                   p.name as product_name, p.slug as product_slug, 
                   pi.image_url as product_image
            FROM carts c
            LEFT JOIN cart_items ci ON c.id = ci.cart_id
            LEFT JOIN products p ON ci.product_id = p.id
            LEFT JOIN (
                SELECT DISTINCT ON (product_id) product_id, image_url
                FROM product_images
                WHERE is_primary = true
                ORDER BY product_id, display_order
            ) pi ON ci.product_id = pi.product_id
            WHERE c.session_id = @SessionId
            ORDER BY ci.created_at ASC";

        return await GetCartWithItems(sql, new { SessionId = sessionId });
    }

    public async Task<Models.Cart> GetOrCreateForUserAsync(Guid userId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart != null)
        {
            return cart;
        }

        var newCart = new Models.Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await CreateAsync(newCart);
    }

    public async Task<Models.Cart> GetOrCreateForSessionAsync(string sessionId)
    {
        var cart = await GetBySessionIdAsync(sessionId);
        if (cart != null)
        {
            return cart;
        }

        var newCart = new Models.Cart
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await CreateAsync(newCart);
    }

    public async Task<Models.Cart> CreateAsync(Models.Cart cart)
    {
        var sql = @"
            INSERT INTO carts (id, user_id, session_id, subtotal, tax, shipping, discount, total, discount_code, created_at, updated_at)
            VALUES (@Id, @UserId, @SessionId, @Subtotal, @Tax, @Shipping, @Discount, @Total, @DiscountCode, @CreatedAt, @UpdatedAt)
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapCart, new
        {
            cart.Id,
            UserId = GetValueOrDbNull(cart.UserId),
            SessionId = GetValueOrDbNull(cart.SessionId),
            cart.Subtotal,
            cart.Tax,
            cart.Shipping,
            cart.Discount,
            cart.Total,
            DiscountCode = GetValueOrDbNull(cart.DiscountCode),
            cart.CreatedAt,
            cart.UpdatedAt
        }) ?? throw new InvalidOperationException("Failed to create cart");
    }

    public async Task<Models.Cart> UpdateAsync(Models.Cart cart)
    {
        var sql = @"
            UPDATE carts 
            SET user_id = @UserId,
                session_id = @SessionId,
                subtotal = @Subtotal,
                tax = @Tax,
                shipping = @Shipping,
                discount = @Discount,
                total = @Total,
                discount_code = @DiscountCode,
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapCart, new
        {
            cart.Id,
            UserId = GetValueOrDbNull(cart.UserId),
            SessionId = GetValueOrDbNull(cart.SessionId),
            cart.Subtotal,
            cart.Tax,
            cart.Shipping,
            cart.Discount,
            cart.Total,
            DiscountCode = GetValueOrDbNull(cart.DiscountCode),
            cart.UpdatedAt
        }) ?? throw new InvalidOperationException("Failed to update cart");
    }

    public async Task DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM carts WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task<List<CartItem>> GetCartItemsAsync(Guid cartId)
    {
        var sql = @"
            SELECT ci.*, p.name as product_name, p.slug as product_slug,
                   pi.image_url as product_image
            FROM cart_items ci
            INNER JOIN products p ON ci.product_id = p.id
            LEFT JOIN (
                SELECT DISTINCT ON (product_id) product_id, image_url
                FROM product_images
                WHERE is_primary = true
                ORDER BY product_id, display_order
            ) pi ON ci.product_id = pi.product_id
            WHERE ci.cart_id = @CartId
            ORDER BY ci.created_at ASC";

        return await QueryAsync(sql, MapCartItem, new { CartId = cartId });
    }

    public async Task<CartItem> AddItemAsync(CartItem item)
    {
        var sql = @"
            INSERT INTO cart_items (id, cart_id, product_id, quantity, price, subtotal, created_at, updated_at)
            VALUES (@Id, @CartId, @ProductId, @Quantity, @Price, @Subtotal, @CreatedAt, @UpdatedAt)
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapCartItemBasic, new
        {
            item.Id,
            item.CartId,
            item.ProductId,
            item.Quantity,
            item.Price,
            item.Subtotal,
            item.CreatedAt,
            item.UpdatedAt
        }) ?? throw new InvalidOperationException("Failed to add cart item");
    }

    public async Task<CartItem> UpdateItemAsync(CartItem item)
    {
        var sql = @"
            UPDATE cart_items 
            SET quantity = @Quantity,
                price = @Price,
                subtotal = @Subtotal,
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapCartItemBasic, new
        {
            item.Id,
            item.Quantity,
            item.Price,
            item.Subtotal,
            item.UpdatedAt
        }) ?? throw new InvalidOperationException("Failed to update cart item");
    }

    public async Task RemoveItemAsync(Guid itemId)
    {
        var sql = "DELETE FROM cart_items WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = itemId });
    }

    public async Task ClearCartAsync(Guid cartId)
    {
        var sql = "DELETE FROM cart_items WHERE cart_id = @CartId";
        await ExecuteAsync(sql, new { CartId = cartId });
    }

    public async Task RecalculateTotalsAsync(Guid cartId)
    {
        var sql = @"
            UPDATE carts
            SET subtotal = (SELECT COALESCE(SUM(subtotal), 0) FROM cart_items WHERE cart_id = @CartId),
                tax = 0,
                shipping = 0,
                total = (SELECT COALESCE(SUM(subtotal), 0) FROM cart_items WHERE cart_id = @CartId),
                updated_at = @UpdatedAt
            WHERE id = @CartId";

        await ExecuteAsync(sql, new { CartId = cartId, UpdatedAt = DateTime.UtcNow });
    }

    private async Task<Models.Cart?> GetCartWithItems(string sql, object parameters)
    {
        Models.Cart? cart = null;
        var items = new Dictionary<Guid, CartItem>();

        await using var connection = await GetConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var properties = parameters.GetType().GetProperties();
        foreach (var prop in properties)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = $"@{prop.Name}";
            parameter.Value = prop.GetValue(parameters) ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            if (cart == null)
            {
                cart = MapCart(reader);
            }

            var itemId = reader["item_id"];
            if (itemId != null && itemId != DBNull.Value)
            {
                var itemGuid = (Guid)itemId;
                if (!items.ContainsKey(itemGuid))
                {
                    var item = MapCartItem(reader);
                    items[itemGuid] = item;
                }
            }
        }

        if (cart != null)
        {
            cart.Items = items.Values.ToList();
        }

        return cart;
    }

    private Models.Cart MapCart(IDataReader reader)
    {
        return new Models.Cart
        {
            Id = (Guid)reader["id"],
            UserId = reader["user_id"] != DBNull.Value ? (Guid?)reader["user_id"] : null,
            SessionId = reader["session_id"] != DBNull.Value ? (string)reader["session_id"] : null,
            Subtotal = (decimal)reader["subtotal"],
            Tax = (decimal)reader["tax"],
            Shipping = (decimal)reader["shipping"],
            Discount = (decimal)reader["discount"],
            Total = (decimal)reader["total"],
            DiscountCode = reader["discount_code"] != DBNull.Value ? (string)reader["discount_code"] : null,
            CreatedAt = (DateTime)reader["created_at"],
            UpdatedAt = (DateTime)reader["updated_at"]
        };
    }

    private CartItem MapCartItem(IDataReader reader)
    {
        var item = new CartItem
        {
            Id = (Guid)reader["item_id"],
            CartId = (Guid)reader["cart_id"],
            ProductId = (Guid)reader["product_id"],
            Quantity = (int)reader["quantity"],
            Price = (decimal)reader["price"],
            Subtotal = (decimal)reader["subtotal"],
            CreatedAt = (DateTime)reader["item_created_at"],
            UpdatedAt = (DateTime)reader["item_updated_at"],
            Product = new Product
            {
                Id = (Guid)reader["product_id"],
                Name = (string)reader["product_name"],
                Slug = (string)reader["product_slug"]
            }
        };

        // Add product image if available
        if (reader["product_image"] != DBNull.Value)
        {
            item.Product.Images = new List<ProductImage>
            {
                new ProductImage
                {
                    ImageUrl = (string)reader["product_image"],
                    IsPrimary = true
                }
            };
        }

        return item;
    }

    private CartItem MapCartItemBasic(IDataReader reader)
    {
        return new CartItem
        {
            Id = (Guid)reader["id"],
            CartId = (Guid)reader["cart_id"],
            ProductId = (Guid)reader["product_id"],
            Quantity = (int)reader["quantity"],
            Price = (decimal)reader["price"],
            Subtotal = (decimal)reader["subtotal"],
            CreatedAt = (DateTime)reader["created_at"],
            UpdatedAt = (DateTime)reader["updated_at"]
        };
    }
}
