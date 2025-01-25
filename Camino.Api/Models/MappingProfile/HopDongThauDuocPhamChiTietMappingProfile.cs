using AutoMapper;
using Camino.Api.Models.HopDongThauDuocPham;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;

namespace Camino.Api.Models.MappingProfile
{
    public class HopDongThauDuocPhamChiTietMappingProfile : Profile
    {
        public HopDongThauDuocPhamChiTietMappingProfile()
        {
            CreateMap<HopDongThauDuocPhamChiTiet, HopDongThauDuocPhamChiTietViewModel>()
                    .AfterMap((source, dest) =>
                    {
                        dest.DuocPham = source.DuocPham != null ? source.DuocPham.Ten : string.Empty;
                    });

            CreateMap<HopDongThauDuocPhamChiTietViewModel, HopDongThauDuocPhamChiTiet>()
                      .ForMember(d => d.HopDongThauDuocPham, o => o.Ignore())
                      .ForMember(d => d.DuocPham, o => o.Ignore());
        }
    }
}
