using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using System.Data;

namespace ECommerce.Core.Reviews;

public class ReviewRepository : BaseRepository, IReviewRepository
{
    public ReviewRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<Review?> GetByIdAsync(Guid id)
    {
        var sql = @"
            SELECT r.*, u.first_name, u.last_name, u.email
            FROM reviews r
            INNER JOIN users u ON r.user_id = u.id
            WHERE r.id = @Id";

        return await QueryFirstOrDefaultAsync(sql, MapReview, new { Id = id });
    }

    public async Task<PaginatedResult<Review>> GetProductReviewsAsync(Guid productId, PaginationFilter filter)
    {
        var whereClause = "WHERE r.product_id = @ProductId AND r.is_approved = true";
        var parameters = new Dictionary<string, object>
        {
            ["ProductId"] = productId
        };

        var countSql = $@"
            SELECT COUNT(*) 
            FROM reviews r 
            {whereClause}";
        
        var totalCount = await ExecuteScalarAsync<int>(countSql, parameters);

        var orderByClause = filter.SortBy?.ToLower() switch
        {
            "rating" => $"r.rating {(filter.IsDescending ? "DESC" : "ASC")}",
            "helpful" => $"r.helpful_count {(filter.IsDescending ? "DESC" : "ASC")}",
            "created" => $"r.created_at {(filter.IsDescending ? "DESC" : "ASC")}",
            _ => "r.created_at DESC"
        };

        var offset = (filter.Page - 1) * filter.PageSize;
        parameters["Offset"] = offset;
        parameters["PageSize"] = filter.PageSize;

        var sql = $@"
            SELECT r.*, u.first_name, u.last_name, u.email
            FROM reviews r
            INNER JOIN users u ON r.user_id = u.id
            {whereClause}
            ORDER BY {orderByClause}
            LIMIT @PageSize OFFSET @Offset";

        var reviews = await QueryAsync(sql, MapReview, parameters);

        return new PaginatedResult<Review>(reviews, totalCount, filter.Page, filter.PageSize);
    }

    public async Task<List<Review>> GetUserReviewsAsync(Guid userId)
    {
        var sql = @"
            SELECT r.*, u.first_name, u.last_name, u.email
            FROM reviews r
            INNER JOIN users u ON r.user_id = u.id
            WHERE r.user_id = @UserId
            ORDER BY r.created_at DESC";

        return await QueryAsync(sql, MapReview, new { UserId = userId });
    }

    public async Task<Review> CreateAsync(Review review)
    {
        var sql = @"
            INSERT INTO reviews (id, product_id, user_id, order_id, rating, title, comment, 
                is_verified_purchase, is_approved, helpful_count, unhelpful_count, created_at, updated_at)
            VALUES (@Id, @ProductId, @UserId, @OrderId, @Rating, @Title, @Comment, 
                @IsVerifiedPurchase, @IsApproved, @HelpfulCount, @UnhelpfulCount, @CreatedAt, @UpdatedAt)
            RETURNING *";

        var result = await QueryFirstOrDefaultAsync(sql, MapReviewWithoutUser, new
        {
            review.Id,
            review.ProductId,
            review.UserId,
            OrderId = GetValueOrDbNull(review.OrderId),
            review.Rating,
            Title = GetValueOrDbNull(review.Title),
            Comment = GetValueOrDbNull(review.Comment),
            review.IsVerifiedPurchase,
            review.IsApproved,
            review.HelpfulCount,
            review.UnhelpfulCount,
            review.CreatedAt,
            review.UpdatedAt
        });

        return result ?? throw new Exception("Failed to create review");
    }

    public async Task<Review> UpdateAsync(Review review)
    {
        var sql = @"
            UPDATE reviews 
            SET rating = @Rating, title = @Title, comment = @Comment, updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *";

        var result = await QueryFirstOrDefaultAsync(sql, MapReviewWithoutUser, new
        {
            review.Id,
            review.Rating,
            Title = GetValueOrDbNull(review.Title),
            Comment = GetValueOrDbNull(review.Comment),
            review.UpdatedAt
        });

        return result ?? throw new Exception("Failed to update review");
    }

    public async Task DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM reviews WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id });
    }

    public async Task<bool> UserHasReviewedProductAsync(Guid userId, Guid productId)
    {
        var sql = @"
            SELECT COUNT(*) 
            FROM reviews 
            WHERE user_id = @UserId AND product_id = @ProductId";

        var count = await ExecuteScalarAsync<int>(sql, new { UserId = userId, ProductId = productId });
        return count > 0;
    }

    private Review MapReview(IDataReader reader)
    {
        var review = MapReviewWithoutUser(reader);
        
        review.User = new User
        {
            Id = review.UserId,
            FirstName = reader.GetString(reader.GetOrdinal("first_name")),
            LastName = reader.GetString(reader.GetOrdinal("last_name")),
            Email = reader.GetString(reader.GetOrdinal("email"))
        };

        return review;
    }

    private Review MapReviewWithoutUser(IDataReader reader)
    {
        return new Review
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            ProductId = reader.GetGuid(reader.GetOrdinal("product_id")),
            UserId = reader.GetGuid(reader.GetOrdinal("user_id")),
            OrderId = reader.IsDBNull(reader.GetOrdinal("order_id")) ? null : reader.GetGuid(reader.GetOrdinal("order_id")),
            Rating = reader.GetInt32(reader.GetOrdinal("rating")),
            Title = reader.IsDBNull(reader.GetOrdinal("title")) ? null : reader.GetString(reader.GetOrdinal("title")),
            Comment = reader.IsDBNull(reader.GetOrdinal("comment")) ? null : reader.GetString(reader.GetOrdinal("comment")),
            IsVerifiedPurchase = reader.GetBoolean(reader.GetOrdinal("is_verified_purchase")),
            IsApproved = reader.GetBoolean(reader.GetOrdinal("is_approved")),
            HelpfulCount = reader.GetInt32(reader.GetOrdinal("helpful_count")),
            UnhelpfulCount = reader.GetInt32(reader.GetOrdinal("unhelpful_count")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}
