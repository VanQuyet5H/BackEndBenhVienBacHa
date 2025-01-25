using AutoMapper;
using Camino.Api.Models.GiayChungNhanPhauThuat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class TinhTrangRaVienHoSoKhacMappingProfile : Profile
    {
        public TinhTrangRaVienHoSoKhacMappingProfile()
        {
            CreateMap<Core.Domain.Entities.TinhTrangRaVienHoSoKhacs.TinhTrangRaVienHoSoKhac, TinhTrangRaVienHoSoKhacViewModel>();
                
            CreateMap<TinhTrangRaVienHoSoKhacViewModel, Core.Domain.Entities.TinhTrangRaVienHoSoKhacs.TinhTrangRaVienHoSoKhac>();
        }
    }
}
