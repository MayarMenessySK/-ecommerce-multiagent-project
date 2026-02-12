using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using System.Data;
using System.Text;

namespace ECommerce.Core.Orders;

public class OrderRepository : BaseRepository, IOrderRepository
{
    public OrderRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<Order?> GetByIdAsync(Guid id)
    {
        var sql = @"
            SELECT o.*, 
                   oi.id as item_id, oi.order_id, oi.product_id, oi.product_name, 
                   oi.product_sku, oi.product_image_url, oi.quantity, 
                   oi.price, oi.subtotal, oi.created_at as item_created_at
            FROM orders o
            LEFT JOIN order_items oi ON o.id = oi.order_id
            WHERE o.id = @Id
            ORDER BY oi.created_at ASC";

        return await GetOrderWithItems(sql, new { Id = id });
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        var sql = @"
            SELECT o.*, 
                   oi.id as item_id, oi.order_id, oi.product_id, oi.product_name, 
                   oi.product_sku, oi.product_image_url, oi.quantity, 
                   oi.price, oi.subtotal, oi.created_at as item_created_at
            FROM orders o
            LEFT JOIN order_items oi ON o.id = oi.order_id
            WHERE o.order_number = @OrderNumber
            ORDER BY oi.created_at ASC";

        return await GetOrderWithItems(sql, new { OrderNumber = orderNumber });
    }

    public async Task<PaginatedResult<Order>> GetUserOrdersAsync(Guid userId, PaginationFilter filter)
    {
        var countSql = "SELECT COUNT(*) FROM orders WHERE user_id = @UserId";
        var totalCount = await ExecuteScalarAsync<int>(countSql, new { UserId = userId });

        var orderBy = filter.SortBy switch
        {
            "orderNumber" => "o.order_number",
            "status" => "o.status",
            "total" => "o.total",
            "createdAt" => "o.created_at",
            _ => "o.created_at"
        };

        var direction = filter.IsDescending ? "DESC" : "ASC";

        var sql = $@"
            SELECT o.*, 
                   oi.id as item_id, oi.order_id, oi.product_id, oi.product_name, 
                   oi.product_sku, oi.product_image_url, oi.quantity, 
                   oi.price, oi.subtotal, oi.created_at as item_created_at
            FROM orders o
            LEFT JOIN order_items oi ON o.id = oi.order_id
            WHERE o.user_id = @UserId
            ORDER BY {orderBy} {direction}, oi.created_at ASC
            LIMIT @Limit OFFSET @Offset";

        var orders = await GetOrdersWithItems(sql, new 
        { 
            UserId = userId,
            Limit = filter.PageSize,
            Offset = filter.Skip
        });

        return new PaginatedResult<Order>
        {
            Items = orders,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<Order> CreateAsync(Order order)
    {
        var sql = @"
            INSERT INTO orders (
                id, order_number, user_id, status, subtotal, tax, shipping_cost, 
                discount, total, currency, discount_code, payment_method, payment_status,
                payment_transaction_id, shipping_method, tracking_number, shipping_carrier,
                shipping_full_name, shipping_phone, shipping_address_line1, shipping_address_line2,
                shipping_city, shipping_state, shipping_postal_code, shipping_country,
                billing_full_name, billing_phone, billing_address_line1, billing_address_line2,
                billing_city, billing_state, billing_postal_code, billing_country,
                notes, estimated_delivery_date, delivered_at, cancelled_at, cancellation_reason,
                created_at, updated_at
            )
            VALUES (
                @Id, @OrderNumber, @UserId, @Status, @Subtotal, @Tax, @ShippingCost,
                @Discount, @Total, @Currency, @DiscountCode, @PaymentMethod, @PaymentStatus,
                @PaymentTransactionId, @ShippingMethod, @TrackingNumber, @ShippingCarrier,
                @ShippingFullName, @ShippingPhone, @ShippingAddressLine1, @ShippingAddressLine2,
                @ShippingCity, @ShippingState, @ShippingPostalCode, @ShippingCountry,
                @BillingFullName, @BillingPhone, @BillingAddressLine1, @BillingAddressLine2,
                @BillingCity, @BillingState, @BillingPostalCode, @BillingCountry,
                @Notes, @EstimatedDeliveryDate, @DeliveredAt, @CancelledAt, @CancellationReason,
                @CreatedAt, @UpdatedAt
            )
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapOrder, new
        {
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.Status,
            order.Subtotal,
            order.Tax,
            order.ShippingCost,
            order.Discount,
            order.Total,
            order.Currency,
            DiscountCode = GetValueOrDbNull(order.DiscountCode),
            order.PaymentMethod,
            order.PaymentStatus,
            PaymentTransactionId = GetValueOrDbNull(order.PaymentTransactionId),
            order.ShippingMethod,
            TrackingNumber = GetValueOrDbNull(order.TrackingNumber),
            ShippingCarrier = GetValueOrDbNull(order.ShippingCarrier),
            order.ShippingFullName,
            order.ShippingPhone,
            order.ShippingAddressLine1,
            ShippingAddressLine2 = GetValueOrDbNull(order.ShippingAddressLine2),
            order.ShippingCity,
            order.ShippingState,
            order.ShippingPostalCode,
            order.ShippingCountry,
            BillingFullName = GetValueOrDbNull(order.BillingFullName),
            BillingPhone = GetValueOrDbNull(order.BillingPhone),
            BillingAddressLine1 = GetValueOrDbNull(order.BillingAddressLine1),
            BillingAddressLine2 = GetValueOrDbNull(order.BillingAddressLine2),
            BillingCity = GetValueOrDbNull(order.BillingCity),
            BillingState = GetValueOrDbNull(order.BillingState),
            BillingPostalCode = GetValueOrDbNull(order.BillingPostalCode),
            BillingCountry = GetValueOrDbNull(order.BillingCountry),
            Notes = GetValueOrDbNull(order.Notes),
            EstimatedDeliveryDate = GetValueOrDbNull(order.EstimatedDeliveryDate),
            DeliveredAt = GetValueOrDbNull(order.DeliveredAt),
            CancelledAt = GetValueOrDbNull(order.CancelledAt),
            CancellationReason = GetValueOrDbNull(order.CancellationReason),
            order.CreatedAt,
            order.UpdatedAt
        }) ?? throw new InvalidOperationException("Failed to create order");
    }

    public async Task<Order> UpdateAsync(Order order)
    {
        var sql = @"
            UPDATE orders 
            SET status = @Status,
                subtotal = @Subtotal,
                tax = @Tax,
                shipping_cost = @ShippingCost,
                discount = @Discount,
                total = @Total,
                discount_code = @DiscountCode,
                payment_status = @PaymentStatus,
                payment_transaction_id = @PaymentTransactionId,
                tracking_number = @TrackingNumber,
                shipping_carrier = @ShippingCarrier,
                estimated_delivery_date = @EstimatedDeliveryDate,
                delivered_at = @DeliveredAt,
                cancelled_at = @CancelledAt,
                cancellation_reason = @CancellationReason,
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapOrder, new
        {
            order.Id,
            order.Status,
            order.Subtotal,
            order.Tax,
            order.ShippingCost,
            order.Discount,
            order.Total,
            DiscountCode = GetValueOrDbNull(order.DiscountCode),
            order.PaymentStatus,
            PaymentTransactionId = GetValueOrDbNull(order.PaymentTransactionId),
            TrackingNumber = GetValueOrDbNull(order.TrackingNumber),
            ShippingCarrier = GetValueOrDbNull(order.ShippingCarrier),
            EstimatedDeliveryDate = GetValueOrDbNull(order.EstimatedDeliveryDate),
            DeliveredAt = GetValueOrDbNull(order.DeliveredAt),
            CancelledAt = GetValueOrDbNull(order.CancelledAt),
            CancellationReason = GetValueOrDbNull(order.CancellationReason),
            order.UpdatedAt
        }) ?? throw new InvalidOperationException("Failed to update order");
    }

    public async Task<Order> CancelAsync(Guid orderId, string cancellationReason)
    {
        var sql = @"
            UPDATE orders 
            SET status = 'Cancelled',
                cancelled_at = @CancelledAt,
                cancellation_reason = @CancellationReason,
                updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapOrder, new
        {
            Id = orderId,
            CancelledAt = DateTime.UtcNow,
            CancellationReason = cancellationReason,
            UpdatedAt = DateTime.UtcNow
        }) ?? throw new InvalidOperationException("Failed to cancel order");
    }

    public async Task<List<OrderItem>> GetOrderItemsAsync(Guid orderId)
    {
        var sql = @"
            SELECT * FROM order_items
            WHERE order_id = @OrderId
            ORDER BY created_at ASC";

        return await QueryAsync(sql, MapOrderItem, new { OrderId = orderId });
    }

    public async Task<OrderItem> AddOrderItemAsync(OrderItem item)
    {
        var sql = @"
            INSERT INTO order_items (
                id, order_id, product_id, product_name, product_sku, 
                product_image_url, quantity, price, subtotal, created_at
            )
            VALUES (
                @Id, @OrderId, @ProductId, @ProductName, @ProductSku,
                @ProductImageUrl, @Quantity, @Price, @Subtotal, @CreatedAt
            )
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapOrderItem, new
        {
            item.Id,
            item.OrderId,
            item.ProductId,
            item.ProductName,
            item.ProductSku,
            ProductImageUrl = GetValueOrDbNull(item.ProductImageUrl),
            item.Quantity,
            item.Price,
            item.Subtotal,
            item.CreatedAt
        }) ?? throw new InvalidOperationException("Failed to add order item");
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, string status)
    {
        var sql = @"
            UPDATE orders 
            SET status = @Status,
                updated_at = @UpdatedAt
            WHERE id = @Id";

        await ExecuteAsync(sql, new
        {
            Id = orderId,
            Status = status,
            UpdatedAt = DateTime.UtcNow
        });
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        var date = DateTime.UtcNow;
        var datePrefix = date.ToString("yyyyMMdd");
        
        var sql = @"
            SELECT COUNT(*) 
            FROM orders 
            WHERE order_number LIKE @Pattern";

        var count = await ExecuteScalarAsync<int>(sql, new { Pattern = $"ORD-{datePrefix}-%" });
        var sequence = (count + 1).ToString("D5");
        
        return $"ORD-{datePrefix}-{sequence}";
    }

    private async Task<Order?> GetOrderWithItems(string sql, object parameters)
    {
        Order? order = null;
        var items = new Dictionary<Guid, OrderItem>();

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
            if (order == null)
            {
                order = MapOrder(reader);
            }

            var itemId = reader["item_id"];
            if (itemId != null && itemId != DBNull.Value)
            {
                var itemGuid = (Guid)itemId;
                if (!items.ContainsKey(itemGuid))
                {
                    var item = MapOrderItemFromJoin(reader);
                    items[itemGuid] = item;
                }
            }
        }

        if (order != null)
        {
            order.Items = items.Values.ToList();
        }

        return order;
    }

    private async Task<List<Order>> GetOrdersWithItems(string sql, object parameters)
    {
        var ordersDict = new Dictionary<Guid, Order>();
        var itemsDict = new Dictionary<Guid, Dictionary<Guid, OrderItem>>();

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
            var orderId = (Guid)reader["id"];
            
            if (!ordersDict.ContainsKey(orderId))
            {
                ordersDict[orderId] = MapOrder(reader);
                itemsDict[orderId] = new Dictionary<Guid, OrderItem>();
            }

            var itemId = reader["item_id"];
            if (itemId != null && itemId != DBNull.Value)
            {
                var itemGuid = (Guid)itemId;
                if (!itemsDict[orderId].ContainsKey(itemGuid))
                {
                    var item = MapOrderItemFromJoin(reader);
                    itemsDict[orderId][itemGuid] = item;
                }
            }
        }

        foreach (var orderId in ordersDict.Keys)
        {
            ordersDict[orderId].Items = itemsDict[orderId].Values.ToList();
        }

        return ordersDict.Values.ToList();
    }

    private Order MapOrder(IDataReader reader)
    {
        return new Order
        {
            Id = (Guid)reader["id"],
            OrderNumber = (string)reader["order_number"],
            UserId = (Guid)reader["user_id"],
            Status = (string)reader["status"],
            Subtotal = (decimal)reader["subtotal"],
            Tax = (decimal)reader["tax"],
            ShippingCost = (decimal)reader["shipping_cost"],
            Discount = (decimal)reader["discount"],
            Total = (decimal)reader["total"],
            Currency = (string)reader["currency"],
            DiscountCode = reader["discount_code"] != DBNull.Value ? (string)reader["discount_code"] : null,
            PaymentMethod = (string)reader["payment_method"],
            PaymentStatus = (string)reader["payment_status"],
            PaymentTransactionId = reader["payment_transaction_id"] != DBNull.Value ? (string)reader["payment_transaction_id"] : null,
            ShippingMethod = (string)reader["shipping_method"],
            TrackingNumber = reader["tracking_number"] != DBNull.Value ? (string)reader["tracking_number"] : null,
            ShippingCarrier = reader["shipping_carrier"] != DBNull.Value ? (string)reader["shipping_carrier"] : null,
            ShippingFullName = (string)reader["shipping_full_name"],
            ShippingPhone = (string)reader["shipping_phone"],
            ShippingAddressLine1 = (string)reader["shipping_address_line1"],
            ShippingAddressLine2 = reader["shipping_address_line2"] != DBNull.Value ? (string)reader["shipping_address_line2"] : null,
            ShippingCity = (string)reader["shipping_city"],
            ShippingState = (string)reader["shipping_state"],
            ShippingPostalCode = (string)reader["shipping_postal_code"],
            ShippingCountry = (string)reader["shipping_country"],
            BillingFullName = reader["billing_full_name"] != DBNull.Value ? (string)reader["billing_full_name"] : null,
            BillingPhone = reader["billing_phone"] != DBNull.Value ? (string)reader["billing_phone"] : null,
            BillingAddressLine1 = reader["billing_address_line1"] != DBNull.Value ? (string)reader["billing_address_line1"] : null,
            BillingAddressLine2 = reader["billing_address_line2"] != DBNull.Value ? (string)reader["billing_address_line2"] : null,
            BillingCity = reader["billing_city"] != DBNull.Value ? (string)reader["billing_city"] : null,
            BillingState = reader["billing_state"] != DBNull.Value ? (string)reader["billing_state"] : null,
            BillingPostalCode = reader["billing_postal_code"] != DBNull.Value ? (string)reader["billing_postal_code"] : null,
            BillingCountry = reader["billing_country"] != DBNull.Value ? (string)reader["billing_country"] : null,
            Notes = reader["notes"] != DBNull.Value ? (string)reader["notes"] : null,
            EstimatedDeliveryDate = reader["estimated_delivery_date"] != DBNull.Value ? (DateTime)reader["estimated_delivery_date"] : null,
            DeliveredAt = reader["delivered_at"] != DBNull.Value ? (DateTime)reader["delivered_at"] : null,
            CancelledAt = reader["cancelled_at"] != DBNull.Value ? (DateTime)reader["cancelled_at"] : null,
            CancellationReason = reader["cancellation_reason"] != DBNull.Value ? (string)reader["cancellation_reason"] : null,
            CreatedAt = (DateTime)reader["created_at"],
            UpdatedAt = (DateTime)reader["updated_at"]
        };
    }

    private OrderItem MapOrderItem(IDataReader reader)
    {
        return new OrderItem
        {
            Id = (Guid)reader["id"],
            OrderId = (Guid)reader["order_id"],
            ProductId = (Guid)reader["product_id"],
            ProductName = (string)reader["product_name"],
            ProductSku = (string)reader["product_sku"],
            ProductImageUrl = reader["product_image_url"] != DBNull.Value ? (string)reader["product_image_url"] : null,
            Quantity = (int)reader["quantity"],
            Price = (decimal)reader["price"],
            Subtotal = (decimal)reader["subtotal"],
            CreatedAt = (DateTime)reader["created_at"]
        };
    }

    private OrderItem MapOrderItemFromJoin(IDataReader reader)
    {
        return new OrderItem
        {
            Id = (Guid)reader["item_id"],
            OrderId = (Guid)reader["order_id"],
            ProductId = (Guid)reader["product_id"],
            ProductName = (string)reader["product_name"],
            ProductSku = (string)reader["product_sku"],
            ProductImageUrl = reader["product_image_url"] != DBNull.Value ? (string)reader["product_image_url"] : null,
            Quantity = (int)reader["quantity"],
            Price = (decimal)reader["price"],
            Subtotal = (decimal)reader["subtotal"],
            CreatedAt = (DateTime)reader["item_created_at"]
        };
    }
}
