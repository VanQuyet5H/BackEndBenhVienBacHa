using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauTraVatTuTuBenhNhan;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauTraVatTuTuBenhNhanMappingProfile : Profile
    {
        public YeuCauTraVatTuTuBenhNhanMappingProfile()
        {
            CreateMap<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan, YeuCauTraVatTuTuBenhNhanViewModel>().IgnoreAllNonExisting()
                .AfterMap((d, s) =>
                {
                    s.TenNhanVienYeuCau = d.NhanVienYeuCau?.User.HoTen;
                    s.TenNhanVienDuyet = d.NhanVienDuyet?.User.HoTen;
                    s.TenKhoTra = d.KhoTra?.Ten;
                    s.TenKhoaTra = d.KhoaHoanTra?.Ten;
                });

            CreateMap<YeuCauTraVatTuTuBenhNhanViewModel, Core.Domain.Entities.YeuCauKhamBenhs.YeuCauTraVatTuTuBenhNhan>().IgnoreAllNonExisting()
                .ForMember(d => d.YeuCauTraVatTuTuBenhNhanChiTiets, o => o.Ignore());
        }
    }
}
