using AutoMapper;
using Domain.Entities;
using Application.DTOs;

namespace Application.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserViewDto>();
            
            CreateMap<User, CreateUserDto>();
            
            CreateMap<CreateUserDto, User>()
                .ForMember(u => u.Guid, opt => opt.MapFrom(_ => Guid.NewGuid()))
                .ForMember(u => u.CreatedOn, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(u => u.CreatedBy, opt => opt.Ignore());
            
            
            CreateMap<UserViewDto, User>();
        }
    }
}