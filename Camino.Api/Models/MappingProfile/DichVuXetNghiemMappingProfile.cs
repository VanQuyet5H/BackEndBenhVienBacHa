using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.DichVuXetNghiem;
using Camino.Core.Domain.Entities.DichVuXetNghiems;
using Camino.Core.Helpers;
using System.Linq;

namespace Camino.Api.Models.MappingProfile
{
    public class DichVuXetNghiemMappingProfile : Profile
    {
        public DichVuXetNghiemMappingProfile()
        {
            CreateMap<Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem, DichVuXetNghiemViewModel>().IgnoreAllNonExisting()

                .AfterMap((s, d) =>
                {
                    d.Ten = s.Ten;
                    d.TenChiSo = s.Ten;
                    d.Ma = s.Ma;
                });

            CreateMap<DichVuXetNghiemViewModel, Core.Domain.Entities.DichVuXetNghiems.DichVuXetNghiem>().IgnoreAllNonExisting()
                 .ForMember(x => x.DichVuKyThuatBenhViens, o => o.Ignore())
                 .ForMember(x => x.DichVuXetNghiemKetNoiChiSos, o => o.Ignore())
                 .ForMember(x => x.KetQuaXetNghiemChiTiets, o => o.Ignore())
                 .ForMember(x => x.KetQuaXetNghiemChiTietChas, o => o.Ignore())
                 .ForMember(x => x.DichVuXetNghiems, o => o.Ignore())
                 ;

            CreateMap<DichVuXetNghiemKetNoiChiSoViewModel, DichVuXetNghiemKetNoiChiSo>().IgnoreAllNonExisting()
                 .ForMember(x => x.KetQuaXetNghiemChiTiets, o => o.Ignore());
        }
    }
}
