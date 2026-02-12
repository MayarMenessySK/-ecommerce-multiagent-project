using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using System.Data;
using System.Text;

namespace ECommerce.Core.Products;

public class ProductRepository : BaseRepository, IProductRepository
{
    public ProductRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        var sql = @"
            SELECT p.*, pi.id as image_id, pi.product_id, pi.image_url, pi.alt_text, 
                   pi.is_primary, pi.display_order, pi.created_at as image_created_at
            FROM products p
            LEFT JOIN product_images pi ON p.id = pi.product_id
            WHERE p.id = @Id
            ORDER BY pi.is_primary DESC, pi.display_order ASC";

        return await GetProductWithImages(sql, new { Id = id });
    }

    public async Task<Product?> GetBySlugAsync(string slug)
    {
        var sql = @"
            SELECT p.*, pi.id as image_id, pi.product_id, pi.image_url, pi.alt_text, 
                   pi.is_primary, pi.display_order, pi.created_at as image_created_at
            FROM products p
            LEFT JOIN product_images pi ON p.id = pi.product_id
            WHERE p.slug = @Slug
            ORDER BY pi.is_primary DESC, pi.display_order ASC";

        return await GetProductWithImages(sql, new { Slug = slug });
    }

    public async Task<PaginatedResult<Product>> GetAllAsync(ProductFilter filter)
    {
        var whereClauses = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (filter.CategoryId.HasValue)
        {
            whereClauses.Add("p.category_id = @CategoryId");
            parameters["CategoryId"] = filter.CategoryId.Value;
        }

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            whereClauses.Add("(LOWER(p.name) LIKE @Search OR LOWER(p.description) LIKE @Search OR LOWER(p.sku) LIKE @Search)");
            parameters["Search"] = $"%{filter.Search.ToLower()}%";
        }

        if (filter.MinPrice.HasValue)
        {
            whereClauses.Add("p.price >= @MinPrice");
            parameters["MinPrice"] = filter.MinPrice.Value;
        }

        if (filter.MaxPrice.HasValue)
        {
            whereClauses.Add("p.price <= @MaxPrice");
            parameters["MaxPrice"] = filter.MaxPrice.Value;
        }

        if (!string.IsNullOrWhiteSpace(filter.Brand))
        {
            whereClauses.Add("LOWER(p.brand) = LOWER(@Brand)");
            parameters["Brand"] = filter.Brand;
        }

        if (filter.IsActive.HasValue)
        {
            whereClauses.Add("p.is_active = @IsActive");
            parameters["IsActive"] = filter.IsActive.Value;
        }

        if (filter.IsFeatured.HasValue)
        {
            whereClauses.Add("p.is_featured = @IsFeatured");
            parameters["IsFeatured"] = filter.IsFeatured.Value;
        }

        if (filter.InStock.HasValue)
        {
            if (filter.InStock.Value)
            {
                whereClauses.Add("p.stock_quantity > 0");
            }
            else
            {
                whereClauses.Add("p.stock_quantity = 0");
            }
        }

        var whereClause = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

        var countSql = $"SELECT COUNT(*) FROM products p {whereClause}";
        var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

        var orderByClause = filter.SortBy?.ToLower() switch
        {
            "name" => $"p.name {(filter.IsDescending ? "DESC" : "ASC")}",
            "price" => $"p.price {(filter.IsDescending ? "DESC" : "ASC")}",
            "rating" => $"p.average_rating {(filter.IsDescending ? "DESC" : "ASC")}",
            "created" => $"p.created_at {(filter.IsDescending ? "DESC" : "ASC")}",
            "sales" => $"p.total_sales {(filter.IsDescending ? "DESC" : "ASC")}",
            _ => "p.created_at DESC"
        };

        var offset = (filter.Page - 1) * filter.PageSize;
        parameters["Offset"] = offset;
        parameters["PageSize"] = filter.PageSize;

        var sql = $@"
            SELECT p.*, pi.id as image_id, pi.product_id, pi.image_url, pi.alt_text, 
                   pi.is_primary, pi.display_order, pi.created_at as image_created_at
            FROM products p
            LEFT JOIN product_images pi ON p.id = pi.product_id
            {whereClause}
            ORDER BY {orderByClause}, pi.is_primary DESC, pi.display_order ASC
            LIMIT @PageSize OFFSET @Offset";

        var products = await GetProductsWithImages(sql, parameters);

        return new PaginatedResult<Product>(products, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        var sql = @"
            INSERT INTO products (id, name, slug, sku, description, short_description, price, 
                original_price, discount_percentage, currency, stock_quantity, low_stock_threshold,
                category_id, brand, weight, dimensions, is_active, is_featured, average_rating,
                total_reviews, total_sales, meta_title, meta_description, meta_keywords, 
                created_at, updated_at)
            VALUES (@Id, @Name, @Slug, @Sku, @Description, @ShortDescription, @Price, 
                @OriginalPrice, @DiscountPercentage, @Currency, @StockQuantity, @LowStockThreshold,
                @CategoryId, @Brand, @Weight, @Dimensions, @IsActive, @IsFeatured, @AverageRating,
                @TotalReviews, @TotalSales, @MetaTitle, @MetaDescription, @MetaKeywords,
                @CreatedAt, @UpdatedAt)
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapProduct, new
        {
            product.Id,
            product.Name,
            product.Slug,
            product.Sku,
            product.Description,
            product.ShortDescription,
            product.Price,
            product.OriginalPrice,
            product.DiscountPercentage,
            product.Currency,
            product.StockQuantity,
            product.LowStockThreshold,
            product.CategoryId,
            product.Brand,
            product.Weight,
            product.Dimensions,
            product.IsActive,
            product.IsFeatured,
            product.AverageRating,
            product.TotalReviews,
            product.TotalSales,
            product.MetaTitle,
            product.MetaDescription,
            product.MetaKeywords,
            product.CreatedAt,
            product.UpdatedAt
        }) ?? throw new Exception("Failed to create product");
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        var sql = @"
            UPDATE products 
            SET name = @Name, slug = @Slug, description = @Description, 
                short_description = @ShortDescription, price = @Price, original_price = @OriginalPrice,
                discount_percentage = @DiscountPercentage, currency = @Currency, 
                stock_quantity = @StockQuantity, low_stock_threshold = @LowStockThreshold,
                category_id = @CategoryId, brand = @Brand, weight = @Weight, dimensions = @Dimensions,
                is_active = @IsActive, is_featured = @IsFeatured, average_rating = @AverageRating,
                total_reviews = @TotalReviews, total_sales = @TotalSales, meta_title = @MetaTitle,
                meta_description = @MetaDescription, meta_keywords = @MetaKeywords, updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapProduct, new
        {
            product.Id,
            product.Name,
            product.Slug,
            product.Description,
            product.ShortDescription,
            product.Price,
            product.OriginalPrice,
            product.DiscountPercentage,
            product.Currency,
            product.StockQuantity,
            product.LowStockThreshold,
            product.CategoryId,
            product.Brand,
            product.Weight,
            product.Dimensions,
            product.IsActive,
            product.IsFeatured,
            product.AverageRating,
            product.TotalReviews,
            product.TotalSales,
            product.MetaTitle,
            product.MetaDescription,
            product.MetaKeywords,
            product.UpdatedAt
        }) ?? throw new Exception("Failed to update product");
    }

    public async Task DeleteAsync(Guid id)
    {
        var sql = "UPDATE products SET is_active = false, updated_at = @UpdatedAt WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
    }

    public async Task<List<ProductImage>> GetProductImagesAsync(Guid productId)
    {
        var sql = @"
            SELECT * FROM product_images 
            WHERE product_id = @ProductId 
            ORDER BY is_primary DESC, display_order ASC";

        return await QueryAsync(sql, MapProductImage, new { ProductId = productId });
    }

    public async Task<ProductImage> AddImageAsync(ProductImage image)
    {
        var sql = @"
            INSERT INTO product_images (id, product_id, image_url, alt_text, is_primary, display_order, created_at)
            VALUES (@Id, @ProductId, @ImageUrl, @AltText, @IsPrimary, @DisplayOrder, @CreatedAt)
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapProductImage, new
        {
            image.Id,
            image.ProductId,
            image.ImageUrl,
            image.AltText,
            image.IsPrimary,
            image.DisplayOrder,
            image.CreatedAt
        }) ?? throw new Exception("Failed to add product image");
    }

    public async Task DeleteImageAsync(Guid imageId)
    {
        var sql = "DELETE FROM product_images WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = imageId });
    }

    public async Task UpdateStockAsync(Guid productId, int quantity)
    {
        var sql = "UPDATE products SET stock_quantity = @Quantity, updated_at = @UpdatedAt WHERE id = @ProductId";
        await ExecuteAsync(sql, new { ProductId = productId, Quantity = quantity, UpdatedAt = DateTime.UtcNow });
    }

    public async Task UpdateRatingAsync(Guid productId)
    {
        var sql = @"
            UPDATE products 
            SET average_rating = (
                SELECT COALESCE(AVG(rating), 0) 
                FROM reviews 
                WHERE product_id = @ProductId AND is_approved = true
            ),
            total_reviews = (
                SELECT COUNT(*) 
                FROM reviews 
                WHERE product_id = @ProductId AND is_approved = true
            ),
            updated_at = @UpdatedAt
            WHERE id = @ProductId";

        await ExecuteAsync(sql, new { ProductId = productId, UpdatedAt = DateTime.UtcNow });
    }

    private async Task<Product?> GetProductWithImages(string sql, object parameters)
    {
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
        
        Product? product = null;
        var images = new List<ProductImage>();

        while (await reader.ReadAsync())
        {
            if (product == null)
            {
                product = MapProduct(reader);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("image_id")))
            {
                images.Add(new ProductImage
                {
                    Id = reader.GetGuid(reader.GetOrdinal("image_id")),
                    ProductId = reader.GetGuid(reader.GetOrdinal("product_id")),
                    ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),
                    AltText = reader.IsDBNull(reader.GetOrdinal("alt_text")) ? null : reader.GetString(reader.GetOrdinal("alt_text")),
                    IsPrimary = reader.GetBoolean(reader.GetOrdinal("is_primary")),
                    DisplayOrder = reader.GetInt32(reader.GetOrdinal("display_order")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("image_created_at"))
                });
            }
        }

        if (product != null)
        {
            product.Images = images;
        }

        return product;
    }

    private async Task<List<Product>> GetProductsWithImages(string sql, Dictionary<string, object> parameters)
    {
        await using var connection = await GetConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        foreach (var param in parameters)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = $"@{param.Key}";
            parameter.Value = param.Value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        await using var reader = await command.ExecuteReaderAsync();
        
        var productDict = new Dictionary<Guid, Product>();

        while (await reader.ReadAsync())
        {
            var productId = reader.GetGuid(reader.GetOrdinal("id"));

            if (!productDict.ContainsKey(productId))
            {
                productDict[productId] = MapProduct(reader);
            }

            if (!reader.IsDBNull(reader.GetOrdinal("image_id")))
            {
                var image = new ProductImage
                {
                    Id = reader.GetGuid(reader.GetOrdinal("image_id")),
                    ProductId = reader.GetGuid(reader.GetOrdinal("product_id")),
                    ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),
                    AltText = reader.IsDBNull(reader.GetOrdinal("alt_text")) ? null : reader.GetString(reader.GetOrdinal("alt_text")),
                    IsPrimary = reader.GetBoolean(reader.GetOrdinal("is_primary")),
                    DisplayOrder = reader.GetInt32(reader.GetOrdinal("display_order")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("image_created_at"))
                };

                if (!productDict[productId].Images.Any(i => i.Id == image.Id))
                {
                    productDict[productId].Images.Add(image);
                }
            }
        }

        return productDict.Values.ToList();
    }

    private Product MapProduct(IDataReader reader)
    {
        return new Product
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Slug = reader.GetString(reader.GetOrdinal("slug")),
            Sku = reader.GetString(reader.GetOrdinal("sku")),
            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
            ShortDescription = reader.IsDBNull(reader.GetOrdinal("short_description")) ? null : reader.GetString(reader.GetOrdinal("short_description")),
            Price = reader.GetDecimal(reader.GetOrdinal("price")),
            OriginalPrice = reader.IsDBNull(reader.GetOrdinal("original_price")) ? null : reader.GetDecimal(reader.GetOrdinal("original_price")),
            DiscountPercentage = reader.IsDBNull(reader.GetOrdinal("discount_percentage")) ? null : reader.GetDecimal(reader.GetOrdinal("discount_percentage")),
            Currency = reader.GetString(reader.GetOrdinal("currency")),
            StockQuantity = reader.GetInt32(reader.GetOrdinal("stock_quantity")),
            LowStockThreshold = reader.GetInt32(reader.GetOrdinal("low_stock_threshold")),
            CategoryId = reader.GetGuid(reader.GetOrdinal("category_id")),
            Brand = reader.IsDBNull(reader.GetOrdinal("brand")) ? null : reader.GetString(reader.GetOrdinal("brand")),
            Weight = reader.IsDBNull(reader.GetOrdinal("weight")) ? null : reader.GetDecimal(reader.GetOrdinal("weight")),
            Dimensions = reader.IsDBNull(reader.GetOrdinal("dimensions")) ? null : reader.GetString(reader.GetOrdinal("dimensions")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            IsFeatured = reader.GetBoolean(reader.GetOrdinal("is_featured")),
            AverageRating = reader.GetDecimal(reader.GetOrdinal("average_rating")),
            TotalReviews = reader.GetInt32(reader.GetOrdinal("total_reviews")),
            TotalSales = reader.GetInt32(reader.GetOrdinal("total_sales")),
            MetaTitle = reader.IsDBNull(reader.GetOrdinal("meta_title")) ? null : reader.GetString(reader.GetOrdinal("meta_title")),
            MetaDescription = reader.IsDBNull(reader.GetOrdinal("meta_description")) ? null : reader.GetString(reader.GetOrdinal("meta_description")),
            MetaKeywords = reader.IsDBNull(reader.GetOrdinal("meta_keywords")) ? null : reader.GetString(reader.GetOrdinal("meta_keywords")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }

    private ProductImage MapProductImage(IDataReader reader)
    {
        return new ProductImage
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            ProductId = reader.GetGuid(reader.GetOrdinal("product_id")),
            ImageUrl = reader.GetString(reader.GetOrdinal("image_url")),
            AltText = reader.IsDBNull(reader.GetOrdinal("alt_text")) ? null : reader.GetString(reader.GetOrdinal("alt_text")),
            IsPrimary = reader.GetBoolean(reader.GetOrdinal("is_primary")),
            DisplayOrder = reader.GetInt32(reader.GetOrdinal("display_order")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at"))
        };
    }
}
