namespace WroclawTheatreTickets.Application.Mapping;

using AutoMapper;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Theatre, TheatreDto>().ReverseMap();
        
        CreateMap<Show, ShowDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.AgeRestriction, opt => opt.MapFrom(s => s.AgeRestriction.ToString()));
        
        CreateMap<Show, ShowDetailDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()))
            .ForMember(d => d.AgeRestriction, opt => opt.MapFrom(s => s.AgeRestriction.ToString()));

        CreateMap<User, UserDto>();
        CreateMap<User, UserProfileDto>()
            .ForMember(d => d.PreferredCategories, opt => opt.MapFrom(s => 
                string.IsNullOrEmpty(s.PreferredCategories) ? new List<string>() : 
                System.Text.Json.JsonSerializer.Deserialize<List<string>>(s.PreferredCategories)));

        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}".Trim() : "Anonymous"));

        CreateMap<Notification, NotificationDto>()
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type.ToString()));
    }
}
