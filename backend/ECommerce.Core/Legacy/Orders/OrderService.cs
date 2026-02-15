using AutoMapper;
using ECommerce.Core.Cart;
using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using ECommerce.Core.Products;

namespace ECommerce.Core.Orders;

public class OrderService : BaseService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<OrderOutput> CreateOrderAsync(CreateOrderInput input, Guid userId)
    {
        ValidateEntity(input);
        ValidateId(userId);

        // Get user's cart
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null || cart.Items.Count == 0)
        {
            ThrowBadRequest("Cart is empty. Cannot create order.");
        }

        // Validate all products are in stock
        foreach (var cartItem in cart.Items)
        {
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            if (product == null)
            {
                ThrowNotFound($"Product {cartItem.ProductId} not found");
            }

            if (!product.IsActive)
            {
                ThrowBadRequest($"Product '{product.Name}' is not available");
            }

            if (product.StockQuantity < cartItem.Quantity)
            {
                ThrowBadRequest($"Insufficient stock for '{product.Name}'. Only {product.StockQuantity} units available");
            }
        }

        // Generate order number
        var orderNumber = await ((OrderRepository)_orderRepository).GenerateOrderNumberAsync();

        // Create order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNumber = orderNumber,
            UserId = userId,
            Status = "Pending",
            PaymentMethod = input.PaymentMethod,
            PaymentStatus = "Pending",
            ShippingMethod = input.ShippingMethod,
            ShippingFullName = input.ShippingAddress.FullName,
            ShippingPhone = input.ShippingAddress.Phone,
            ShippingAddressLine1 = input.ShippingAddress.AddressLine1,
            ShippingAddressLine2 = input.ShippingAddress.AddressLine2,
            ShippingCity = input.ShippingAddress.City,
            ShippingState = input.ShippingAddress.State,
            ShippingPostalCode = input.ShippingAddress.PostalCode,
            ShippingCountry = input.ShippingAddress.Country,
            Notes = input.Notes,
            DiscountCode = input.DiscountCode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Set billing address if provided, otherwise use shipping address
        if (input.BillingAddress != null)
        {
            order.BillingFullName = input.BillingAddress.FullName;
            order.BillingPhone = input.BillingAddress.Phone;
            order.BillingAddressLine1 = input.BillingAddress.AddressLine1;
            order.BillingAddressLine2 = input.BillingAddress.AddressLine2;
            order.BillingCity = input.BillingAddress.City;
            order.BillingState = input.BillingAddress.State;
            order.BillingPostalCode = input.BillingAddress.PostalCode;
            order.BillingCountry = input.BillingAddress.Country;
        }
        else
        {
            order.BillingFullName = order.ShippingFullName;
            order.BillingPhone = order.ShippingPhone;
            order.BillingAddressLine1 = order.ShippingAddressLine1;
            order.BillingAddressLine2 = order.ShippingAddressLine2;
            order.BillingCity = order.ShippingCity;
            order.BillingState = order.ShippingState;
            order.BillingPostalCode = order.ShippingPostalCode;
            order.BillingCountry = order.ShippingCountry;
        }

        // Calculate totals
        decimal subtotal = 0;
        foreach (var cartItem in cart.Items)
        {
            subtotal += cartItem.Subtotal;
        }

        // Calculate shipping cost (simple logic - can be enhanced)
        decimal shippingCost = input.ShippingMethod.ToLower() switch
        {
            "standard" => 5.00m,
            "express" => 15.00m,
            "overnight" => 25.00m,
            _ => 0m
        };

        // Calculate tax (simple 10% tax - can be enhanced based on location)
        decimal tax = subtotal * 0.10m;

        order.Subtotal = subtotal;
        order.Tax = tax;
        order.ShippingCost = shippingCost;
        order.Discount = 0; // Can be enhanced with discount code logic
        order.Total = subtotal + tax + shippingCost - order.Discount;

        // Estimate delivery date
        order.EstimatedDeliveryDate = input.ShippingMethod.ToLower() switch
        {
            "standard" => DateTime.UtcNow.AddDays(7),
            "express" => DateTime.UtcNow.AddDays(3),
            "overnight" => DateTime.UtcNow.AddDays(1),
            _ => DateTime.UtcNow.AddDays(7)
        };

        // Create order in database
        var createdOrder = await _orderRepository.CreateAsync(order);

        // Create order items from cart items
        foreach (var cartItem in cart.Items)
        {
            var product = await _productRepository.GetByIdAsync(cartItem.ProductId);
            
            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = createdOrder.Id,
                ProductId = cartItem.ProductId,
                ProductName = product!.Name,
                ProductSku = product.Sku,
                ProductImageUrl = product.Images.FirstOrDefault()?.ImageUrl,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
                Subtotal = cartItem.Subtotal,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepository.AddOrderItemAsync(orderItem);

            // Reduce product stock
            await _productRepository.UpdateStockAsync(cartItem.ProductId, product.StockQuantity - cartItem.Quantity);
        }

        // Clear the cart after order is created
        await _cartRepository.ClearCartAsync(cart.Id);

        // Fetch the complete order with items
        var completeOrder = await _orderRepository.GetByIdAsync(createdOrder.Id);
        return _mapper.Map<OrderOutput>(completeOrder);
    }

    public async Task<OrderOutput> GetOrderAsync(Guid orderId, Guid userId)
    {
        ValidateId(orderId);
        ValidateId(userId);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            ThrowNotFound("Order", orderId);
        }

        // Verify order belongs to user
        if (order.UserId != userId)
        {
            ThrowUnauthorized("You do not have permission to view this order");
        }

        return _mapper.Map<OrderOutput>(order);
    }

    public async Task<OrderOutput> GetOrderByNumberAsync(string orderNumber, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
        {
            throw new ArgumentException("Order number is required", nameof(orderNumber));
        }

        ValidateId(userId);

        var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
        if (order == null)
        {
            ThrowNotFound($"Order with number {orderNumber} not found");
        }

        // Verify order belongs to user
        if (order.UserId != userId)
        {
            ThrowUnauthorized("You do not have permission to view this order");
        }

        return _mapper.Map<OrderOutput>(order);
    }

    public async Task<PaginatedResult<OrderListOutput>> GetUserOrdersAsync(Guid userId, PaginationFilter filter)
    {
        ValidateId(userId);

        var result = await _orderRepository.GetUserOrdersAsync(userId, filter);
        
        var orderListOutputs = result.Items.Select(order => new OrderListOutput
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status,
            Total = order.Total,
            Currency = order.Currency,
            PaymentMethod = order.PaymentMethod,
            PaymentStatus = order.PaymentStatus,
            ShippingMethod = order.ShippingMethod,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            ItemCount = order.Items.Count
        }).ToList();

        return new PaginatedResult<OrderListOutput>
        {
            Items = orderListOutputs,
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }

    public async Task<OrderOutput> CancelOrderAsync(Guid orderId, CancelOrderInput input, Guid userId)
    {
        ValidateId(orderId);
        ValidateEntity(input);
        ValidateId(userId);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            ThrowNotFound("Order", orderId);
        }

        // Verify order belongs to user
        if (order.UserId != userId)
        {
            ThrowUnauthorized("You do not have permission to cancel this order");
        }

        // Only allow cancellation if status is Pending or Processing
        if (order.Status != "Pending" && order.Status != "Processing")
        {
            ThrowBadRequest($"Cannot cancel order with status '{order.Status}'. Only Pending or Processing orders can be cancelled.");
        }

        // Restore product stock for all items
        foreach (var item in order.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                await _productRepository.UpdateStockAsync(item.ProductId, product.StockQuantity + item.Quantity);
            }
        }

        // Cancel the order
        var cancelledOrder = await _orderRepository.CancelAsync(orderId, input.CancellationReason);

        // Fetch the complete order with items
        var completeOrder = await _orderRepository.GetByIdAsync(cancelledOrder.Id);
        return _mapper.Map<OrderOutput>(completeOrder);
    }

    public async Task<OrderOutput> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusInput input, Guid userId)
    {
        ValidateId(orderId);
        ValidateEntity(input);
        ValidateId(userId);

        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            ThrowNotFound("Order", orderId);
        }

        // Verify order belongs to user (or add admin check)
        if (order.UserId != userId)
        {
            ThrowUnauthorized("You do not have permission to update this order");
        }

        // Update status
        await _orderRepository.UpdateOrderStatusAsync(orderId, input.Status);

        // If status is Delivered, set delivered timestamp
        if (input.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
        {
            order.DeliveredAt = DateTime.UtcNow;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
        }

        // Fetch the updated order
        var updatedOrder = await _orderRepository.GetByIdAsync(orderId);
        return _mapper.Map<OrderOutput>(updatedOrder);
    }
}
