using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.Users;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.Users;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<Role, RoleViewModel>().IgnoreAllNonExisting();
            CreateMap<RoleViewModel, Role>().IgnoreAllNonExisting()
                .ForMember(d => d.RoleFunctions, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.UserType = Enums.UserType.NhanVien;
                });
        }
    }
}
