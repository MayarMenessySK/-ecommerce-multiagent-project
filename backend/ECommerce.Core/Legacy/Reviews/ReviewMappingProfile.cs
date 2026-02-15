using AutoMapper;
using ECommerce.Core.Models;

namespace ECommerce.Core.Reviews;

public class ReviewMappingProfile : Profile
{
    public ReviewMappingProfile()
    {
        CreateMap<Review, ReviewOutput>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => 
                src.User != null ? $"{src.User.FirstName} {src.User.LastName}".Trim() : string.Empty));

        CreateMap<CreateReviewInput, Review>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.IsVerifiedPurchase, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.HelpfulCount, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.UnhelpfulCount, opt => opt.MapFrom(_ => 0))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.User, opt => opt.Ignore());

        CreateMap<UpdateReviewInput, Review>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ProductId, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.OrderId, opt => opt.Ignore())
            .ForMember(dest => dest.IsVerifiedPurchase, opt => opt.Ignore())
            .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
            .ForMember(dest => dest.HelpfulCount, opt => opt.Ignore())
            .ForMember(dest => dest.UnhelpfulCount, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}
