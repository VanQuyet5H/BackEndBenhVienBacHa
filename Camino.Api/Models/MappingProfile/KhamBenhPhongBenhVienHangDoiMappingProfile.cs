using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamBenh;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamBenhPhongBenhVienHangDoiMappingProfile: Profile
    {
        public KhamBenhPhongBenhVienHangDoiMappingProfile()
        {
            CreateMap<KhamBenhPhongBenhVienHangDoiViewModel, PhongBenhVienHangDoi>().IgnoreAllNonExisting();
            CreateMap<PhongBenhVienHangDoi, KhamBenhPhongBenhVienHangDoiViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauTiepNhan, o => o.MapFrom(y => y.YeuCauTiepNhan))
                .ForMember(x => x.YeuCauKhamBenh, o => o.MapFrom(y => y.YeuCauKhamBenh))
                ;
        }
    }
}
