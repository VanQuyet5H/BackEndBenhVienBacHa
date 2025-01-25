using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauTraDuocPhamTuBenhNhan;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauTraDuocPhamTuBenhNhanMappingProfile : Profile
    {
        public YeuCauTraDuocPhamTuBenhNhanMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraDuocPhamTuBenhNhan, YeuCauTraDuocPhamTuBenhNhanViewModel>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    s.TenNhanVienYeuCau = d.NhanVienYeuCau?.User.HoTen;
                    s.TenNhanVienDuyet = d.NhanVienDuyet?.User.HoTen;
                    s.TenKhoTra = d.KhoTra?.Ten;
                    s.TenKhoaTra = d.KhoaHoanTra?.Ten;
                });

            CreateMap<YeuCauTraDuocPhamTuBenhNhanViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraDuocPhamTuBenhNhan>().IgnoreAllNonExisting()
                .ForMember(d => d.YeuCauTraDuocPhamTuBenhNhanChiTiets, o => o.Ignore());
        }
    }
}
