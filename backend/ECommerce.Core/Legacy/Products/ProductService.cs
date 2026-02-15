using AutoMapper;
using ECommerce.Core.Misc;
using ECommerce.Core.Models;

namespace ECommerce.Core.Products;

public class ProductService : BaseService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductOutput> CreateProductAsync(CreateProductInput input)
    {
        ValidateEntity(input);

        var product = _mapper.Map<Product>(input);
        product.Slug = input.Name.ToSlug();

        product = await _productRepository.CreateAsync(product);
        return _mapper.Map<ProductOutput>(product);
    }

    public async Task<ProductOutput> UpdateProductAsync(Guid id, UpdateProductInput input)
    {
        ValidateId(id);
        ValidateEntity(input);

        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            ThrowNotFound("Product", id);
        }

        var oldName = product!.Name;
        _mapper.Map(input, product);

        if (oldName != input.Name)
        {
            product.Slug = input.Name.ToSlug();
        }

        product.UpdatedAt = DateTime.UtcNow;

        product = await _productRepository.UpdateAsync(product);
        return _mapper.Map<ProductOutput>(product);
    }

    public async Task<ProductOutput> GetProductAsync(Guid id)
    {
        ValidateId(id);

        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            ThrowNotFound("Product", id);
        }

        return _mapper.Map<ProductOutput>(product!);
    }

    public async Task<ProductOutput> GetProductBySlugAsync(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            ThrowBadRequest("Slug cannot be empty");
        }

        var product = await _productRepository.GetBySlugAsync(slug);
        if (product == null)
        {
            ThrowNotFound($"Product with slug '{slug}' not found");
        }

        return _mapper.Map<ProductOutput>(product!);
    }

    public async Task<PaginatedResult<ProductListOutput>> GetProductsAsync(ProductFilter filter)
    {
        var result = await _productRepository.GetAllAsync(filter);
        
        var listOutputs = result.Items.Select(p => _mapper.Map<ProductListOutput>(p)).ToList();

        return new PaginatedResult<ProductListOutput>(
            listOutputs,
            result.TotalCount,
            result.Page,
            result.PageSize
        );
    }

    public async Task DeleteProductAsync(Guid id)
    {
        ValidateId(id);

        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            ThrowNotFound("Product", id);
        }

        await _productRepository.DeleteAsync(id);
    }

    public async Task<ProductImageOutput> AddProductImageAsync(AddProductImageInput input)
    {
        ValidateEntity(input);
        ValidateId(input.ProductId);

        var product = await _productRepository.GetByIdAsync(input.ProductId);
        if (product == null)
        {
            ThrowNotFound("Product", input.ProductId);
        }

        var existingImages = await _productRepository.GetProductImagesAsync(input.ProductId);
        
        var displayOrder = existingImages.Count > 0 ? existingImages.Max(i => i.DisplayOrder) + 1 : 0;

        var image = new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = input.ProductId,
            ImageUrl = input.ImageUrl,
            AltText = input.AltText,
            IsPrimary = input.IsPrimary,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow
        };

        image = await _productRepository.AddImageAsync(image);
        return _mapper.Map<ProductImageOutput>(image);
    }

    public async Task DeleteProductImageAsync(Guid imageId)
    {
        ValidateId(imageId);
        await _productRepository.DeleteImageAsync(imageId);
    }
}
