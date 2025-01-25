using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauHoanTraVatTu;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject.DanhSachYeuCauHoanTra.VatTu;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauHoanTraVatTuMappingProfile : Profile
    {
        public YeuCauHoanTraVatTuMappingProfile()
        {
            CreateMap<YeuCauTraVatTu, YeuCauHoanTraVatTuViewModel>()
                .ForMember(d => d.YeuCauTraVatTuChiTiets, o => o.MapFrom(s => s.YeuCauTraVatTuChiTiets))
                .AfterMap((s, d) =>
                {
                    d.KhoXuat = s.KhoXuat.Ten;
                    d.KhoNhap = s.KhoNhap.Ten;
                    d.NhanVienDuyet = s.NhanVienDuyet?.User?.HoTen;
                    d.NhanVienYeuCau = s.NhanVienYeuCau?.User?.HoTen;
                }).IgnoreAllNonExisting();

            CreateMap<YeuCauHoanTraVatTuViewModel, YeuCauTraVatTu>()
                .ForMember(d => d.YeuCauTraVatTuChiTiets, o => o.Ignore());

            CreateMap<YeuCauTraVatTuChiTiet, YeuCauTraVatTuChiTietViewModel>()
                .AfterMap((s, d) =>
                {
                    d.HopDong = s.HopDongThauVatTu.NhaThau.Ten;
                    d.VatTu = s.VatTuBenhVien.VatTus.Ten;
                    d.Nhom = s.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription();
                }).IgnoreAllNonExisting();

            CreateMap<YeuCauTraVatTuChiTietViewModel, YeuCauTraVatTuChiTiet>().IgnoreAllNonExisting();

            CreateMap<DanhSachYeuCauHoanTraVatTuGridVo, DanhSachYeuCauHoanTraVatTuExportExcel>()
                .IgnoreAllNonExisting();
            CreateMap<DanhSachYeuCauHoanTraVatTuChiTietGridVo, DanhSachYeuCauHoanTraVatTuChiTietExportExcelChild>().IgnoreAllNonExisting();

            //Update 31/12/2021
            CreateMap<YeuCauTraVatTu, YeuCauHoanTraVatTuTuTrucViewModel>().IgnoreAllNonExisting()
                  .ForMember(x => x.YeuCauHoanTraVatTuChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoNhap = s.KhoNhap?.Ten;
                    d.TenKhoXuat = s.KhoXuat?.Ten;
                    d.TenNhanVienYeuCau = s.NhanVienYeuCau?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                });
            CreateMap<YeuCauHoanTraVatTuTuTrucViewModel, YeuCauTraVatTu>().IgnoreAllNonExisting()
                ;
        }
    }
}
