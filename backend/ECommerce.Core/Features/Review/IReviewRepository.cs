namespace ECommerce.Core.Features.Review;

public interface IReviewRepository
{
    Task<ReviewEntity?> GetByIdAsync(Guid id);
    Task<List<ReviewEntity>> GetByProductIdAsync(Guid productId);
    Task<List<ReviewEntity>> GetByUserIdAsync(Guid userId);
    Task<ReviewEntity> CreateAsync(ReviewEntity review);
    Task<ReviewEntity> UpdateAsync(ReviewEntity review);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ApproveAsync(Guid id);
}
