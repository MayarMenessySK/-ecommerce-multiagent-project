namespace ECommerce.Core.Features.Category;

public interface ICategoryRepository
{
    Task<CategoryEntity?> GetByIdAsync(Guid id);
    Task<CategoryEntity?> GetBySlugAsync(string slug);
    Task<List<CategoryEntity>> GetAllAsync();
    Task<List<CategoryEntity>> GetActiveAsync();
    Task<CategoryEntity> CreateAsync(CategoryEntity category);
    Task<CategoryEntity> UpdateAsync(CategoryEntity category);
    Task<bool> DeleteAsync(Guid id);
}
