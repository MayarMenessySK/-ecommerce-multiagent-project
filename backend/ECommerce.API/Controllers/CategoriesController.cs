using ECommerce.API.Models;
using ECommerce.Core.Categories;
using ECommerce.Core.Misc;
using ECommerce.Core.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/v1/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;

    public CategoriesController(
        ILogger<CategoriesController> logger,
        ICategoryRepository categoryRepository,
        IProductRepository productRepository)
    {
        _logger = logger;
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CategoryOutput>>>> GetCategories()
    {
        try
        {
            // TODO: Implement get hierarchical categories logic
            var categories = new List<CategoryOutput>();

            var response = new ApiResponse<List<CategoryOutput>>
            {
                Success = true,
                Data = categories,
                Message = "Categories retrieved successfully"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving categories");

            var errorResponse = new ApiErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "GET_CATEGORIES_ERROR",
                    Message = ex.Message,
                    Details = null
                }
            };

            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CategoryOutput>>> GetCategoryById(Guid id)
    {
        try
        {
            // TODO: Implement get category by ID logic
            var category = new CategoryOutput();

            var response = new ApiResponse<CategoryOutput>
            {
                Success = true,
                Data = category,
                Message = "Category retrieved successfully"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category with ID {CategoryId}", id);

            var errorResponse = new ApiErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "GET_CATEGORY_ERROR",
                    Message = ex.Message,
                    Details = null
                }
            };

            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<ApiResponse<CategoryOutput>>> GetCategoryBySlug(string slug)
    {
        try
        {
            // TODO: Implement get category by slug logic
            var category = new CategoryOutput();

            var response = new ApiResponse<CategoryOutput>
            {
                Success = true,
                Data = category,
                Message = "Category retrieved successfully"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving category with slug {Slug}", slug);

            var errorResponse = new ApiErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "GET_CATEGORY_ERROR",
                    Message = ex.Message,
                    Details = null
                }
            };

            return StatusCode(500, errorResponse);
        }
    }

    [HttpGet("{id}/products")]
    public async Task<ActionResult<ApiResponse<PaginatedResult<ProductListOutput>>>> GetCategoryProducts(
        Guid id, 
        [FromQuery] ProductFilter filter)
    {
        try
        {
            // TODO: Implement get category products logic
            // Ensure the filter includes the category ID
            filter.CategoryId = id;
            var result = new PaginatedResult<ProductListOutput>();

            var response = new ApiResponse<PaginatedResult<ProductListOutput>>
            {
                Success = true,
                Data = result,
                Message = "Category products retrieved successfully"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving products for category {CategoryId}", id);

            var errorResponse = new ApiErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "GET_CATEGORY_PRODUCTS_ERROR",
                    Message = ex.Message,
                    Details = null
                }
            };

            return StatusCode(500, errorResponse);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CategoryOutput>>> CreateCategory([FromBody] CreateCategoryInput input)
    {
        try
        {
            // TODO: Implement create category logic
            var category = new CategoryOutput();

            var response = new ApiResponse<CategoryOutput>
            {
                Success = true,
                Data = category,
                Message = "Category created successfully"
            };

            return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating category");

            var errorResponse = new ApiErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "CREATE_CATEGORY_ERROR",
                    Message = ex.Message,
                    Details = null
                }
            };

            return StatusCode(500, errorResponse);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CategoryOutput>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryInput input)
    {
        try
        {
            // TODO: Implement update category logic
            var category = new CategoryOutput();

            var response = new ApiResponse<CategoryOutput>
            {
                Success = true,
                Data = category,
                Message = "Category updated successfully"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating category with ID {CategoryId}", id);

            var errorResponse = new ApiErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "UPDATE_CATEGORY_ERROR",
                    Message = ex.Message,
                    Details = null
                }
            };

            return StatusCode(500, errorResponse);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteCategory(Guid id)
    {
        try
        {
            // TODO: Implement delete category logic

            var response = new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "Category deleted successfully"
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting category with ID {CategoryId}", id);

            var errorResponse = new ApiErrorResponse
            {
                Error = new ErrorDetail
                {
                    Code = "DELETE_CATEGORY_ERROR",
                    Message = ex.Message,
                    Details = null
                }
            };

            return StatusCode(500, errorResponse);
        }
    }
}
