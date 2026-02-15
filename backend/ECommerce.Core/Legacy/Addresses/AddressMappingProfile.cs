using AutoMapper;
using ECommerce.Core.Models;

namespace ECommerce.Core.Addresses;

public class AddressMappingProfile : Profile
{
    public AddressMappingProfile()
    {
        CreateMap<Address, AddressOutput>()
            .ForMember(dest => dest.AddressId, opt => opt.MapFrom(src => src.Id));

        CreateMap<CreateAddressInput, Address>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UpdateAddressInput, Address>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}
