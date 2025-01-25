using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KetQuaSinhHieu;
using Camino.Api.Models.PhauThuatThuThuat;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class KetQuaSinhHieuMappingProfile:Profile
    {
        public KetQuaSinhHieuMappingProfile()
        {
            CreateMap<Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu, KetQuaSinhHieuViewModel>()
                .IgnoreAllNonExisting()
                .AfterMap((source, dest) =>
                {
                    dest.BMI = source.Bmi;
                    dest.NgayThucHien = source.LastTime.Value.ApplyFormatDateTimeSACH();
                });
            CreateMap<KetQuaSinhHieuViewModel, Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu>()
                .IgnoreAllNonExisting()
                .AfterMap((source, dest) =>
                {
                    dest.Bmi = source.BMI;
                });
            CreateMap<ChiSoSinhHieuViewModel, Core.Domain.Entities.KetQuaSinhHieus.KetQuaSinhHieu>()
                .IgnoreAllNonExisting()
                .AfterMap((s, d) => { d.LastTime = s.NgayThucHien; });
        }
    }
}
