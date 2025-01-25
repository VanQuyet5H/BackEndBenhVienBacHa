using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.KhamTheoDoi;

namespace Camino.Api.Models.MappingProfile
{
    public class KhamTheoDoiMappingProfile : Profile
    {
        public KhamTheoDoiMappingProfile()
        {
            CreateMap<Core.Domain.Entities.PhauThuatThuThuats.KhamTheoDoi, KhamTheoDoiViewModel>().IgnoreAllNonExisting();

            CreateMap<KhamTheoDoiViewModel, Core.Domain.Entities.PhauThuatThuThuats.KhamTheoDoi>().IgnoreAllNonExisting()
                .ForMember(p => p.KhamTheoDoiBoPhanKhacs, o => o.Ignore());
        }
    }
}
