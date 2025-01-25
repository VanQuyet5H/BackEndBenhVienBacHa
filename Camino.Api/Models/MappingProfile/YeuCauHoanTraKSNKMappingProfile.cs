using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.YeuCauHoanTraKSNK;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.KhoKSNKs;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class YeuCauHoanTraKSNKMappingProfile : Profile
    {
        public YeuCauHoanTraKSNKMappingProfile()
        {
            CreateMap<YeuCauTraVatTu, YeuCauHoanTraKSNKViewModel>()
                .ForMember(d => d.YeuCauTraKSNKChiTiets, o => o.MapFrom(s => s.YeuCauTraVatTuChiTiets))
                .AfterMap((s, d) =>
                {
                    d.KhoXuat = s.KhoXuat.Ten;
                    d.KhoNhap = s.KhoNhap.Ten;
                    d.NhanVienDuyet = s.NhanVienDuyet?.User?.HoTen;
                    d.NhanVienYeuCau = s.NhanVienYeuCau?.User?.HoTen;
                }).IgnoreAllNonExisting();

            CreateMap<YeuCauHoanTraKSNKViewModel, YeuCauTraVatTu>()
                .ForMember(d => d.YeuCauTraVatTuChiTiets, o => o.Ignore());

            CreateMap<YeuCauTraVatTuChiTiet, YeuCauTraKSNKChiTietViewModel>()
                .AfterMap((s, d) =>
                {
                    d.HopDong = s.HopDongThauVatTu.NhaThau.Ten;
                    d.VatTu = s.VatTuBenhVien.VatTus.Ten;
                    d.Nhom = s.VatTuBenhVien.LoaiSuDung.GetValueOrDefault().GetDescription();
                }).IgnoreAllNonExisting();

            CreateMap<YeuCauTraKSNKChiTietViewModel, YeuCauTraVatTuChiTiet>().IgnoreAllNonExisting();

            CreateMap<DanhSachYeuCauHoanTraKSNKGridVo, DanhSachYeuCauHoanTraKSNKExportExcel>()
                .IgnoreAllNonExisting();
            CreateMap<DanhSachYCHoanTraKSNKChiTietGridVo, DanhSachYeuCauHoanTraKSNKChiTietExportExcelChild>().IgnoreAllNonExisting();

            //Update 31/12/2021
            CreateMap<YeuCauTraVatTu, YeuCauHoanTraKSNKTuTrucViewModel>().IgnoreAllNonExisting()
                  .ForMember(x => x.YeuCauHoanTraKSNKChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoNhap = s.KhoNhap?.Ten;
                    d.TenKhoXuat = s.KhoXuat?.Ten;
                    d.TenNhanVienYeuCau = s.NhanVienYeuCau?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                });
            CreateMap<YeuCauTraDuocPham, YeuCauHoanTraKSNKTuTrucViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauHoanTraKSNKChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoNhap = s.KhoNhap?.Ten;
                    d.TenKhoXuat = s.KhoXuat?.Ten;
                    d.TenNhanVienYeuCau = s.NhanVienYeuCau?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                });

            CreateMap<YeuCauHoanTraKSNKTuTrucViewModel, YeuCauTraVatTu>().IgnoreAllNonExisting()
                ;
            CreateMap<YeuCauHoanTraKSNKTuTrucViewModel, YeuCauTraDuocPham>().IgnoreAllNonExisting()
                ;
        }
    }
}
