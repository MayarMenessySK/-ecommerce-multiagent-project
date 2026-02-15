using AutoMapper;
using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using ECommerce.Core.Products;

namespace ECommerce.Core.Reviews;

public class ReviewService : BaseService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly string _connectionString;

    public ReviewService(
        IReviewRepository reviewRepository,
        IProductRepository productRepository,
        IMapper mapper,
        string connectionString)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _connectionString = connectionString;
    }

    public async Task<ReviewOutput> CreateReviewAsync(Guid userId, CreateReviewInput input)
    {
        ValidateEntity(input);

        if (input.Rating < 1 || input.Rating > 5)
        {
            ThrowBadRequest("Rating must be between 1 and 5");
        }

        var hasReviewed = await _reviewRepository.UserHasReviewedProductAsync(userId, input.ProductId);
        if (hasReviewed)
        {
            ThrowBadRequest("You have already reviewed this product");
        }

        var product = await _productRepository.GetByIdAsync(input.ProductId);
        if (product == null)
        {
            ThrowNotFound("Product", input.ProductId);
        }

        var isVerifiedPurchase = await CheckVerifiedPurchaseAsync(userId, input.ProductId);

        var review = _mapper.Map<Review>(input);
        review.UserId = userId;
        review.IsVerifiedPurchase = isVerifiedPurchase;

        review = await _reviewRepository.CreateAsync(review);

        await _productRepository.UpdateRatingAsync(input.ProductId);

        var createdReview = await _reviewRepository.GetByIdAsync(review.Id);
        return _mapper.Map<ReviewOutput>(createdReview!);
    }

    public async Task<ReviewOutput> UpdateReviewAsync(Guid userId, Guid reviewId, UpdateReviewInput input)
    {
        ValidateId(reviewId);
        ValidateEntity(input);

        if (input.Rating < 1 || input.Rating > 5)
        {
            ThrowBadRequest("Rating must be between 1 and 5");
        }

        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
        {
            ThrowNotFound("Review", reviewId);
        }

        if (review.UserId != userId)
        {
            ThrowUnauthorized("You can only update your own reviews");
        }

        var productId = review.ProductId;

        _mapper.Map(input, review);
        review.UpdatedAt = DateTime.UtcNow;

        review = await _reviewRepository.UpdateAsync(review);

        await _productRepository.UpdateRatingAsync(productId);

        var updatedReview = await _reviewRepository.GetByIdAsync(review.Id);
        return _mapper.Map<ReviewOutput>(updatedReview!);
    }

    public async Task DeleteReviewAsync(Guid userId, Guid reviewId)
    {
        ValidateId(reviewId);

        var review = await _reviewRepository.GetByIdAsync(reviewId);
        if (review == null)
        {
            ThrowNotFound("Review", reviewId);
        }

        if (review.UserId != userId)
        {
            ThrowUnauthorized("You can only delete your own reviews");
        }

        var productId = review.ProductId;

        await _reviewRepository.DeleteAsync(reviewId);

        await _productRepository.UpdateRatingAsync(productId);
    }

    public async Task<PaginatedResult<ReviewOutput>> GetProductReviewsAsync(Guid productId, PaginationFilter filter)
    {
        ValidateId(productId);

        var result = await _reviewRepository.GetProductReviewsAsync(productId, filter);
        
        var reviewOutputs = result.Items.Select(r => _mapper.Map<ReviewOutput>(r)).ToList();
        
        return new PaginatedResult<ReviewOutput>(
            reviewOutputs,
            result.TotalCount,
            result.Page,
            result.PageSize
        );
    }

    public async Task<List<ReviewOutput>> GetUserReviewsAsync(Guid userId)
    {
        ValidateId(userId);

        var reviews = await _reviewRepository.GetUserReviewsAsync(userId);
        return reviews.Select(r => _mapper.Map<ReviewOutput>(r)).ToList();
    }

    private async Task<bool> CheckVerifiedPurchaseAsync(Guid userId, Guid productId)
    {
        var sql = @"
            SELECT COUNT(*) 
            FROM order_items oi
            INNER JOIN orders o ON oi.order_id = o.id
            WHERE o.user_id = @UserId 
                AND oi.product_id = @ProductId
                AND o.status IN ('Completed', 'Delivered')";

        await using var connection = await GetConnectionAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var userIdParam = command.CreateParameter();
        userIdParam.ParameterName = "@UserId";
        userIdParam.Value = userId;
        command.Parameters.Add(userIdParam);

        var productIdParam = command.CreateParameter();
        productIdParam.ParameterName = "@ProductId";
        productIdParam.Value = productId;
        command.Parameters.Add(productIdParam);

        var result = await command.ExecuteScalarAsync();
        var count = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
        
        return count > 0;
    }

    private async Task<Npgsql.NpgsqlConnection> GetConnectionAsync()
    {
        var connection = new Npgsql.NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
