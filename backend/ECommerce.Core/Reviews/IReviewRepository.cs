using ECommerce.Core.Misc;
using ECommerce.Core.Models;

namespace ECommerce.Core.Reviews;

public interface IReviewRepository
{
    Task<Review?> GetByIdAsync(Guid id);
    Task<PaginatedResult<Review>> GetProductReviewsAsync(Guid productId, PaginationFilter filter);
    Task<List<Review>> GetUserReviewsAsync(Guid userId);
    Task<Review> CreateAsync(Review review);
    Task<Review> UpdateAsync(Review review);
    Task DeleteAsync(Guid id);
    Task<bool> UserHasReviewedProductAsync(Guid userId, Guid productId);
}
