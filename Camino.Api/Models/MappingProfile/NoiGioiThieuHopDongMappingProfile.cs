using AutoMapper;
using Camino.Api.Models.CauhinhHeSoTheoNoiGioiThieuHoaHong;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class NoiGioiThieuHopDongMappingProfile : Profile
    {
        public NoiGioiThieuHopDongMappingProfile()
        {
            CreateMap<NoiGioiThieuHopDong, NoiGioiThieuHopDongViewModel>();
            CreateMap<NoiGioiThieuHopDongViewModel, NoiGioiThieuHopDong>();
           
        }

    }
}
