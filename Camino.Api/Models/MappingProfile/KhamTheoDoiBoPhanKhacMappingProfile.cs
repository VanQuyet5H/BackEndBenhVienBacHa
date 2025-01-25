using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamTheoDoiBoPhanKhac;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamTheoDoiBoPhanKhacMappingProfile : Profile
    {
        public KhamTheoDoiBoPhanKhacMappingProfile()
        {
            CreateMap<Core.Domain.Entities.PhauThuatThuThuats.KhamTheoDoiBoPhanKhac, KhamTheoDoiBoPhanKhacViewModel >().IgnoreAllNonExisting();

            CreateMap<KhamTheoDoiBoPhanKhacViewModel, Core.Domain.Entities.PhauThuatThuThuats.KhamTheoDoiBoPhanKhac>().IgnoreAllNonExisting();
        }
    }
}
