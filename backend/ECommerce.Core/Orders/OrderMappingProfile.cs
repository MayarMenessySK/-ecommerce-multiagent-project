using AutoMapper;
using ECommerce.Core.Models;

namespace ECommerce.Core.Orders;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderOutput>()
            .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => new ShippingAddressOutput
            {
                FullName = src.ShippingFullName,
                Phone = src.ShippingPhone,
                AddressLine1 = src.ShippingAddressLine1,
                AddressLine2 = src.ShippingAddressLine2,
                City = src.ShippingCity,
                State = src.ShippingState,
                PostalCode = src.ShippingPostalCode,
                Country = src.ShippingCountry
            }))
            .ForMember(dest => dest.BillingAddress, opt => opt.MapFrom(src => 
                !string.IsNullOrEmpty(src.BillingFullName) 
                    ? new BillingAddressOutput
                    {
                        FullName = src.BillingFullName!,
                        Phone = src.BillingPhone!,
                        AddressLine1 = src.BillingAddressLine1!,
                        AddressLine2 = src.BillingAddressLine2,
                        City = src.BillingCity!,
                        State = src.BillingState!,
                        PostalCode = src.BillingPostalCode!,
                        Country = src.BillingCountry!
                    }
                    : null))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));

        CreateMap<Order, OrderListOutput>()
            .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.Items.Count));

        CreateMap<OrderItem, OrderItemOutput>();

        CreateMap<CreateOrderInput, Order>()
            .ForMember(dest => dest.ShippingFullName, opt => opt.MapFrom(src => src.ShippingAddress.FullName))
            .ForMember(dest => dest.ShippingPhone, opt => opt.MapFrom(src => src.ShippingAddress.Phone))
            .ForMember(dest => dest.ShippingAddressLine1, opt => opt.MapFrom(src => src.ShippingAddress.AddressLine1))
            .ForMember(dest => dest.ShippingAddressLine2, opt => opt.MapFrom(src => src.ShippingAddress.AddressLine2))
            .ForMember(dest => dest.ShippingCity, opt => opt.MapFrom(src => src.ShippingAddress.City))
            .ForMember(dest => dest.ShippingState, opt => opt.MapFrom(src => src.ShippingAddress.State))
            .ForMember(dest => dest.ShippingPostalCode, opt => opt.MapFrom(src => src.ShippingAddress.PostalCode))
            .ForMember(dest => dest.ShippingCountry, opt => opt.MapFrom(src => src.ShippingAddress.Country))
            .ForMember(dest => dest.BillingFullName, opt => opt.MapFrom(src => 
                src.BillingAddress != null ? src.BillingAddress.FullName : null))
            .ForMember(dest => dest.BillingPhone, opt => opt.MapFrom(src => 
                src.BillingAddress != null ? src.BillingAddress.Phone : null))
            .ForMember(dest => dest.BillingAddressLine1, opt => opt.MapFrom(src => 
                src.BillingAddress != null ? src.BillingAddress.AddressLine1 : null))
            .ForMember(dest => dest.BillingAddressLine2, opt => opt.MapFrom(src => 
                src.BillingAddress != null ? src.BillingAddress.AddressLine2 : null))
            .ForMember(dest => dest.BillingCity, opt => opt.MapFrom(src => 
                src.BillingAddress != null ? src.BillingAddress.City : null))
            .ForMember(dest => dest.BillingState, opt => opt.MapFrom(src => 
                src.BillingAddress != null ? src.BillingAddress.State : null))
            .ForMember(dest => dest.BillingPostalCode, opt => opt.MapFrom(src => 
                src.BillingAddress != null ? src.BillingAddress.PostalCode : null))
            .ForMember(dest => dest.BillingCountry, opt => opt.MapFrom(src => 
                src.BillingAddress != null ? src.BillingAddress.Country : null));
    }
}
