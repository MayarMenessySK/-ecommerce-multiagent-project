using AutoMapper;
using ECommerce.Core.Models;

namespace ECommerce.Core.Cart;

public class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        CreateMap<Models.Cart, CartOutput>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<CartItem, CartItemOutput>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => 
                src.Product != null ? src.Product.Name : string.Empty))
            .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => 
                src.Product != null && src.Product.Images.Any() 
                    ? src.Product.Images.First().ImageUrl 
                    : null));
    }
}
