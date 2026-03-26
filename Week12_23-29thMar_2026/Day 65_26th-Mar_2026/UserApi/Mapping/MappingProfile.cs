using AutoMapper;
using UserApi.Models;
using UserApi.DTOs;

namespace UserApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>();
            CreateMap<RegisterDTO, User>();
        }
    }
}