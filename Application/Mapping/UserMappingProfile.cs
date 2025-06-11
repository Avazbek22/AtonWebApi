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
        }
    }
}