using System.Linq;
using AutoMapper;
using Camino.Api.Extensions;
using Camino.Api.Models.XuatKhos;
using Camino.Api.Models.YeuCauDieuChuyenKhoThuoc;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauDieuChuyenDuocPhams;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.XuatKhos;
using Camino.Core.Domain.ValueObject.YeuCauDieuChuyenDuocPhams;
using Camino.Core.Helpers;

namespace Camino.Api.Models.MappingProfile
{
    public class XuatKhoDieuChuyenKhoNoiBoDuocPhamMappingProfile : Profile
    {
        public XuatKhoDieuChuyenKhoNoiBoDuocPhamMappingProfile()
        {
            CreateMap<YeuCauDieuChuyenDuocPham, XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauDieuChuyenDuocPhamChiTiets, o => o.Ignore())
                .AfterMap((s, d) =>
                {
                    d.TenKhoNhap = s.KhoNhap?.Ten;
                    d.TenKhoXuat = s.KhoXuat?.Ten;
                    d.TenNguoiNhap = s.NguoiNhap?.User.HoTen;
                    d.TenNguoiXuat = s.NguoiXuat?.User.HoTen;
                    d.TenNhanVienDuyet = s.NhanVienDuyet?.User.HoTen;
                });
            CreateMap<XuatKhoDieuChuyenKhoNoiBoDuocPhamViewModel, YeuCauDieuChuyenDuocPham>().IgnoreAllNonExisting()
                .ForMember(x => x.YeuCauDieuChuyenDuocPhamChiTiets, o => o.Ignore())
                ;

            CreateMap<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietViewModel, XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo>().IgnoreAllNonExisting();
            CreateMap<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo, XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietViewModel>().IgnoreAllNonExisting();

            CreateMap<XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo, YeuCauDieuChuyenDuocPhamChiTiet>().IgnoreAllNonExisting();
            CreateMap<YeuCauDieuChuyenDuocPhamChiTiet, XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo>().IgnoreAllNonExisting();

            CreateMap<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo, YeuCauDieuChuyenDuocPhamChiTiet>().IgnoreAllNonExisting();
            CreateMap<YeuCauDieuChuyenDuocPhamChiTiet, YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo>().IgnoreAllNonExisting();

            #region Excel

            CreateMap<YeuCauDieuChuyenDuocPhamVo, YeuCauDieuChuyenThuocExportExcel>();
            CreateMap<YeuCauDieuChuyenDuocPhamChiTietVo, YeuCauDieuChuyenThuocExportExcelChild>();
            #endregion

        }
    }
}
