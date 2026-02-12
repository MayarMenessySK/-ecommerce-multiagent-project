using AutoMapper;
using ECommerce.Core.Models;

namespace ECommerce.Core.Products;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductOutput>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
            .ForMember(dest => dest.IsInStock, opt => opt.MapFrom(src => src.IsInStock))
            .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock));

        CreateMap<Product, ProductListOutput>()
            .ForMember(dest => dest.IsInStock, opt => opt.MapFrom(src => src.IsInStock))
            .ForMember(dest => dest.PrimaryImage, opt => opt.MapFrom(src => 
                src.Images.FirstOrDefault(i => i.IsPrimary) != null 
                    ? src.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl 
                    : src.Images.FirstOrDefault() != null 
                        ? src.Images.FirstOrDefault()!.ImageUrl 
                        : null));

        CreateMap<ProductImage, ProductImageOutput>();

        CreateMap<CreateProductInput, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.TotalSales, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.Images, opt => opt.Ignore());

        CreateMap<UpdateProductInput, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.Sku, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.AverageRating, opt => opt.Ignore())
            .ForMember(dest => dest.TotalReviews, opt => opt.Ignore())
            .ForMember(dest => dest.TotalSales, opt => opt.Ignore())
            .ForMember(dest => dest.Images, opt => opt.Ignore())
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
