using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Users;
using Camino.Core.Domain.Entities.Users;

namespace Camino.Api.Models.MappingProfile
{
    public class RoleFunctionMappingProfile : Profile
    {
        public RoleFunctionMappingProfile()
        {

            CreateMap<RoleFunction, RoleFunctionViewModel>().IgnoreAllNonExisting();
            CreateMap<RoleFunctionViewModel, RoleFunction>().IgnoreAllNonExisting();
        }
    }
}