using ECommerce.Core.Misc;
using ECommerce.Core.Models;
using System.Data;

namespace ECommerce.Core.Categories;

public class CategoryRepository : BaseRepository, ICategoryRepository
{
    public CategoryRepository(string connectionString) : base(connectionString)
    {
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        var sql = "SELECT * FROM categories WHERE id = @Id";
        return await QueryFirstOrDefaultAsync(sql, MapCategory, new { Id = id });
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        var sql = "SELECT * FROM categories WHERE slug = @Slug";
        return await QueryFirstOrDefaultAsync(sql, MapCategory, new { Slug = slug });
    }

    public async Task<List<Category>> GetAllAsync()
    {
        var sql = "SELECT * FROM categories ORDER BY level, display_order, name";
        var categories = await QueryAsync(sql, MapCategory, null);
        
        return BuildHierarchy(categories);
    }

    public async Task<List<Category>> GetParentCategoriesAsync()
    {
        var sql = @"
            SELECT * FROM categories 
            WHERE parent_category_id IS NULL 
            ORDER BY display_order, name";
        
        return await QueryAsync(sql, MapCategory, null);
    }

    public async Task<List<Category>> GetSubCategoriesAsync(Guid parentId)
    {
        var sql = @"
            SELECT * FROM categories 
            WHERE parent_category_id = @ParentId 
            ORDER BY display_order, name";
        
        return await QueryAsync(sql, MapCategory, new { ParentId = parentId });
    }

    public async Task<Category> CreateAsync(Category category)
    {
        var sql = @"
            INSERT INTO categories (id, name, slug, description, image_url, parent_category_id, 
                level, is_active, display_order, created_at, updated_at)
            VALUES (@Id, @Name, @Slug, @Description, @ImageUrl, @ParentCategoryId, 
                @Level, @IsActive, @DisplayOrder, @CreatedAt, @UpdatedAt)
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapCategory, new
        {
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.ImageUrl,
            category.ParentCategoryId,
            category.Level,
            category.IsActive,
            category.DisplayOrder,
            category.CreatedAt,
            category.UpdatedAt
        }) ?? throw new Exception("Failed to create category");
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        var sql = @"
            UPDATE categories 
            SET name = @Name, slug = @Slug, description = @Description, image_url = @ImageUrl,
                parent_category_id = @ParentCategoryId, level = @Level, is_active = @IsActive,
                display_order = @DisplayOrder, updated_at = @UpdatedAt
            WHERE id = @Id
            RETURNING *";

        return await QueryFirstOrDefaultAsync(sql, MapCategory, new
        {
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.ImageUrl,
            category.ParentCategoryId,
            category.Level,
            category.IsActive,
            category.DisplayOrder,
            category.UpdatedAt
        }) ?? throw new Exception("Failed to update category");
    }

    public async Task DeleteAsync(Guid id)
    {
        var sql = "UPDATE categories SET is_active = false, updated_at = @UpdatedAt WHERE id = @Id";
        await ExecuteAsync(sql, new { Id = id, UpdatedAt = DateTime.UtcNow });
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        var sql = "SELECT COUNT(*) FROM categories WHERE slug = @Slug";
        var count = await ExecuteScalarAsync<int>(sql, new { Slug = slug });
        return count > 0;
    }

    public async Task<bool> SlugExistsExceptAsync(string slug, Guid categoryId)
    {
        var sql = "SELECT COUNT(*) FROM categories WHERE slug = @Slug AND id != @CategoryId";
        var count = await ExecuteScalarAsync<int>(sql, new { Slug = slug, CategoryId = categoryId });
        return count > 0;
    }

    private List<Category> BuildHierarchy(List<Category> allCategories)
    {
        var categoryDict = allCategories.ToDictionary(c => c.Id);
        var rootCategories = new List<Category>();

        foreach (var category in allCategories)
        {
            if (category.ParentCategoryId == null)
            {
                rootCategories.Add(category);
            }
            else if (categoryDict.TryGetValue(category.ParentCategoryId.Value, out var parent))
            {
                parent.SubCategories.Add(category);
            }
        }

        return rootCategories;
    }

    private Category MapCategory(IDataReader reader)
    {
        return new Category
        {
            Id = reader.GetGuid(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Slug = reader.GetString(reader.GetOrdinal("slug")),
            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
            ImageUrl = reader.IsDBNull(reader.GetOrdinal("image_url")) ? null : reader.GetString(reader.GetOrdinal("image_url")),
            ParentCategoryId = reader.IsDBNull(reader.GetOrdinal("parent_category_id")) ? null : reader.GetGuid(reader.GetOrdinal("parent_category_id")),
            Level = reader.GetInt32(reader.GetOrdinal("level")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            DisplayOrder = reader.GetInt32(reader.GetOrdinal("display_order")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at"))
        };
    }
}
