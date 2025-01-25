using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauKhamBenhBoPhanTonThuongMappingProfile : Profile
    {
        public YeuCauKhamBenhBoPhanTonThuongMappingProfile()
        {
            CreateMap<YeuCauKhamBenhBoPhanTonThuongViewModel, YeuCauKhamBenhBoPhanTonThuong>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauKhamBenh, o => o.Ignore());
            CreateMap<YeuCauKhamBenhBoPhanTonThuong, YeuCauKhamBenhBoPhanTonThuongViewModel>().IgnoreAllNonExisting();
        }
    }
}
