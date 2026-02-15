using ECommerce.Core.Models;

namespace ECommerce.Core.Categories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetBySlugAsync(string slug);
    Task<List<Category>> GetAllAsync();
    Task<List<Category>> GetParentCategoriesAsync();
    Task<List<Category>> GetSubCategoriesAsync(Guid parentId);
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
    Task<bool> SlugExistsAsync(string slug);
    Task<bool> SlugExistsExceptAsync(string slug, Guid categoryId);
}
