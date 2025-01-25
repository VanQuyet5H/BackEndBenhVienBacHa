using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.NoiGioiThieu;
using Camino.Core.Domain.Entities.NoiGioiThieu;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiGioiThieuChiTietMienGiamMappingProfile : Profile
    {
        public NoiGioiThieuChiTietMienGiamMappingProfile()
        {
            CreateMap<NoiGioiThieuChiTietMienGiamViewModel, NoiGioiThieuChiTietMienGiam>().IgnoreAllNonExisting();
            CreateMap<NoiGioiThieuChiTietMienGiam, NoiGioiThieuChiTietMienGiamViewModel>().IgnoreAllNonExisting();
        }
    }
}
