using AutoMapper;
using ECommerce.Core.Misc;
using ECommerce.Core.Models;

namespace ECommerce.Core.Categories;

public class CategoryService : BaseService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryOutput> CreateCategoryAsync(CreateCategoryInput input)
    {
        ValidateEntity(input);

        var slug = input.Name.ToSlug();
        
        if (await _categoryRepository.SlugExistsAsync(slug))
        {
            ThrowBadRequest($"A category with slug '{slug}' already exists");
        }

        int level = 0;
        if (input.ParentCategoryId.HasValue)
        {
            var parent = await _categoryRepository.GetByIdAsync(input.ParentCategoryId.Value);
            if (parent == null)
            {
                ThrowNotFound("Parent category", input.ParentCategoryId.Value);
            }
            level = parent!.Level + 1;
        }

        var category = _mapper.Map<Category>(input);
        category.Slug = slug;
        category.Level = level;

        category = await _categoryRepository.CreateAsync(category);
        return _mapper.Map<CategoryOutput>(category);
    }

    public async Task<CategoryOutput> UpdateCategoryAsync(Guid id, UpdateCategoryInput input)
    {
        ValidateId(id);
        ValidateEntity(input);

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            ThrowNotFound("Category", id);
        }

        var oldName = category!.Name;
        _mapper.Map(input, category);

        if (oldName != input.Name)
        {
            var slug = input.Name.ToSlug();
            if (await _categoryRepository.SlugExistsExceptAsync(slug, id))
            {
                ThrowBadRequest($"A category with slug '{slug}' already exists");
            }
            category.Slug = slug;
        }

        int level = 0;
        if (input.ParentCategoryId.HasValue)
        {
            if (input.ParentCategoryId.Value == id)
            {
                ThrowBadRequest("A category cannot be its own parent");
            }

            var parent = await _categoryRepository.GetByIdAsync(input.ParentCategoryId.Value);
            if (parent == null)
            {
                ThrowNotFound("Parent category", input.ParentCategoryId.Value);
            }

            if (await IsDescendant(id, input.ParentCategoryId.Value))
            {
                ThrowBadRequest("Cannot set a descendant category as parent (would create circular reference)");
            }

            level = parent!.Level + 1;
        }
        
        category.Level = level;
        category.UpdatedAt = DateTime.UtcNow;

        category = await _categoryRepository.UpdateAsync(category);
        return _mapper.Map<CategoryOutput>(category);
    }

    public async Task<CategoryOutput> GetCategoryAsync(Guid id)
    {
        ValidateId(id);

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            ThrowNotFound("Category", id);
        }

        return _mapper.Map<CategoryOutput>(category!);
    }

    public async Task<CategoryOutput> GetCategoryBySlugAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            ThrowBadRequest("Slug cannot be empty");
        }

        var category = await _categoryRepository.GetBySlugAsync(slug);
        if (category == null)
        {
            ThrowNotFound($"Category with slug '{slug}' not found");
        }

        return _mapper.Map<CategoryOutput>(category!);
    }

    public async Task<List<CategoryOutput>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return _mapper.Map<List<CategoryOutput>>(categories);
    }

    public async Task<List<CategoryListOutput>> GetParentCategoriesAsync()
    {
        var categories = await _categoryRepository.GetParentCategoriesAsync();
        return _mapper.Map<List<CategoryListOutput>>(categories);
    }

    public async Task<List<CategoryListOutput>> GetSubCategoriesAsync(Guid parentId)
    {
        ValidateId(parentId);

        var parent = await _categoryRepository.GetByIdAsync(parentId);
        if (parent == null)
        {
            ThrowNotFound("Parent category", parentId);
        }

        var categories = await _categoryRepository.GetSubCategoriesAsync(parentId);
        return _mapper.Map<List<CategoryListOutput>>(categories);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        ValidateId(id);

        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
        {
            ThrowNotFound("Category", id);
        }

        var subCategories = await _categoryRepository.GetSubCategoriesAsync(id);
        if (subCategories.Count > 0)
        {
            ThrowBadRequest("Cannot delete a category that has subcategories");
        }

        await _categoryRepository.DeleteAsync(id);
    }

    private async Task<bool> IsDescendant(Guid categoryId, Guid potentialAncestorId)
    {
        var subCategories = await _categoryRepository.GetSubCategoriesAsync(categoryId);
        
        foreach (var subCategory in subCategories)
        {
            if (subCategory.Id == potentialAncestorId)
            {
                return true;
            }

            if (await IsDescendant(subCategory.Id, potentialAncestorId))
            {
                return true;
            }
        }

        return false;
    }
}
