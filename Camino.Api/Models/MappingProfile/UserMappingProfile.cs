using AutoMapper;
using Camino.Api.Models.Users;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Api.Models.MappingProfile
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserViewModel>();
            CreateMap<UserViewModel,User>();
        }
    }
}
