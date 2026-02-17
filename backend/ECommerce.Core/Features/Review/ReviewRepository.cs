namespace ECommerce.Core.Features.Review;

public class ReviewRepository : BaseRepository, IReviewRepository
{
    public ReviewRepository(DataAccessAdapter adapter) : base(adapter)
    {
    }

    public async Task<ReviewEntity?> GetByIdAsync(Guid id)
    {
        var query = _meta.Review.Where(r => r.Id == id);
        return await ExecuteQuerySingleAsync(query);
    }

    public async Task<List<ReviewEntity>> GetByProductIdAsync(Guid productId)
    {
        var query = _meta.Review
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreatedAt);
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<List<ReviewEntity>> GetByUserIdAsync(Guid userId)
    {
        var query = _meta.Review
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt);
        
        return await ExecuteQueryAsync(query);
    }

    public async Task<ReviewEntity> CreateAsync(ReviewEntity review)
    {
        review.Id = Guid.NewGuid();
        review.CreatedAt = DateTime.UtcNow;
        review.UpdatedAt = DateTime.UtcNow;
        
        await SaveAsync(review);
        return review;
    }

    public async Task<ReviewEntity> UpdateAsync(ReviewEntity review)
    {
        review.UpdatedAt = DateTime.UtcNow;
        await SaveAsync(review);
        return review;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var review = await GetByIdAsync(id);
        if (review == null) return false;
        
        return await DeleteAsync(review);
    }

    public async Task<bool> ApproveAsync(Guid id)
    {
        var review = await GetByIdAsync(id);
        if (review == null) return false;
        
        review.IsApproved = true;
        review.UpdatedAt = DateTime.UtcNow;
        return await SaveAsync(review);
    }
}
