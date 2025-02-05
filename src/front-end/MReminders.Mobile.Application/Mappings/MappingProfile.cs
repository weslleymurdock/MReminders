using AutoMapper;
using MReminders.Mobile.Application.Requests.Account;
using MReminders.Mobile.Domain.Entities;

namespace MReminders.Mobile.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterRequest, AppUser>()
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
        .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
        .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow))
        .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}