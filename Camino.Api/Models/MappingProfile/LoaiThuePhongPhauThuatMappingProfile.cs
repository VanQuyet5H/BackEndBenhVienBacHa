using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.LoaiThuePhongPhauThuat;

namespace Camino.Api.Models.MappingProfile
{
    public class LoaiThuePhongPhauThuatMappingProfile : Profile
    {
        public LoaiThuePhongPhauThuatMappingProfile()
        {
            CreateMap<Core.Domain.Entities.CauHinhs.LoaiThuePhongPhauThuat, LoaiThuePhongPhauThuatViewModel>()
                .IgnoreAllNonExisting();
            CreateMap<LoaiThuePhongPhauThuatViewModel, Core.Domain.Entities.CauHinhs.LoaiThuePhongPhauThuat>()
                .IgnoreAllNonExisting();
        }
    }
}
