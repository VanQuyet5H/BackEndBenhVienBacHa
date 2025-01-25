using AutoMapper;
using Camino.Api.Models.YeuCauKhamBenhChanDoanPhanBiet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauKhamBenhChanDoanPhanBietMappingProfile : Profile
    {
        public YeuCauKhamBenhChanDoanPhanBietMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet, YeuCauKhamBenhChanDoanPhanBietViewModel>()
                .AfterMap((s, d) =>
                {
                    //d.TenICD = s.ICD?.Ma + " - " + s.ICD?.TenTiengViet;
                    d.TenICD = s.ICD != null ? s.ICD.Ma + " - " + s.ICD.TenTiengViet : "";

                });
            CreateMap<YeuCauKhamBenhChanDoanPhanBietViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenhChanDoanPhanBiet>();
        }
    }
}
