using System;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.TiemChungs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;
using Camino.Core.Domain.ValueObject.CauHinh;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Services.TiemChung
{
    public partial interface ITiemChungService
    {
        #region get grid
        Task<List<KhamBenhGoiDichVuGridVo>> GetGridDichVuKyThuatTiemChung(GridChiDinhVuKyThuatTiemChungQueryInfoVo queryInfo);

        Task<GridDataSource> GetDataForGridHoanThanhTiemChungAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetDataForGridHoanThanhTiemChungAsyncVer2(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridHoanThanhTiemChungAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGridHoanThanhTiemChungAsyncVer2(QueryInfo queryInfo);
        #endregion

        #region get data
        List<LookupItemVo> GetKetLuans(DropDownListRequestModel queryInfo);
        List<LookupItemVo> GetViTriTiems(DropDownListRequestModel queryInfo);
        Task<string> GetTemplateKhamSangLocTiemChungAsync(string ten);
        Task<List<VacxinTiemChungVo>> GetVaccinesAsync(DropDownListRequestModel queryInfo);
        Task<int> GetSoThuTuTiepTheoTrongHangDoiTiemChungAsync(long phongBenhVienId);
        Task<LookupItemVo> GetBacSiThucHienMacDinhAsync(long noiThucHienId);
        #endregion

        #region Kiểm tra data
        void KiemTraKetLuanPhuHopVoiHuongDan(YeuCauDichVuKyThuat yeuCauDichVuKyThuat);
        Task KiemTraSoLuongConLaiCuaDichVuTrongGoiAsync(TiemChungChiDinhGoiDichVuTheoBenhNhanVo yeuCauVo);
        Task<bool> KiemTraDangKyGoiDichVuTheoBenhNhanAsync(long benhNhanId);
        #endregion

        #region xử lý data
        Task KiemTraDatayeuCauTiemChungAsync(long yeuCauDichVuKyThuatId, long phongBenhVienHangDoiId = 0, Enums.EnumTrangThaiHangDoi trangThaiHangDoi = Enums.EnumTrangThaiHangDoi.DangKham, long? yeuCauDichVuKyThuatVacxinId = null);
        Task<bool> KiemTraCoBenhNhanKhacDangKhamTiemChungTrongPhong(long? yeuCauDichVuKyThuatId, long phongBenhVienId, Enums.LoaiHangDoiTiemVacxin loaiHangDoi = Enums.LoaiHangDoiTiemVacxin.KhamSangLoc);
        Task XuLyHoanThanhCongDoanKhamTiemChungHienTaiCuaBenhNhan(long yeuCauDichVuKyThuatId, bool hoanThanhKham = false, long? yeuCauDichVuKyThuatVacxinId = null);
        Task<YeuCauDichVuKyThuat> ThemChiDinhVacxinAsync(YeuCauKhamTiemChungVo yeuCauVo, YeuCauTiepNhan yeuCauTiepNhan);
        Task XuLySoLuongChiDinhVacxinAsync(ICollection<YeuCauDichVuKyThuat> yeuCauDichVuKyThuats, List<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTiets);
        Task XuLyThemSoLuongChiDinhVacxinAsync(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, List<NhapChiTietVacxinTiemChungVo> nhapChiTietVacxin, CauHinhChung cauHinhChung, long khoVacXinId);
        Task XuLyXoaSoLuongChiDinhVacxinAsync(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, List<NhapChiTietVacxinTiemChungVo> nhapChiTietVacxin);
        Task XuLyTiemChungQuayLaiChuaKhamAsync(long yeuCauDichVuKyThuatId);
        #endregion

        #region In
        Task<string> InBanKiemTruocTiemChungDoiVoiTreEm(long yeuCauDichVuKyThuatKhamSangLocId, string hosting);
        #endregion

        #region Gói dịch vụ
        Task<List<TiemChungChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo>> GetThongTinSuDungDichVuGiuongTrongGoiAsync(long benhNhanId);
        Task<GridDataSource> GetGoiDichVuCuaBenhNhanDataForGridAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetGoiDichVuCuaBenhNhanTotalPageForGridAsync(QueryInfo queryInfo);
        Task<TiemChungGridDataSourceChiTietGoiDichVuTheoBenhNhan> GetChiTietGoiDichVuCuaBenhNhanDataForGridAsync(QueryInfo queryInfo, bool isDieuTriNoiTru = false, List<TiemChungChiTietSuDungDichVuGiuongTrongGoiTheoBenhNhanVo> dichVuGiuongDaChiDinhs = null);
        Task<TiemChungGridDataSourceChiTietGoiDichVuTheoBenhNhan> GetChiTietGoiDichVuCuaBenhNhanTotalPageForGridAsync(QueryInfo queryInfo, bool isDieuTriNoiTru = false);
        Task XuLyThemChiDinhGoiDichVuTheoBenhNhanAsync(YeuCauTiepNhan yeuCauTiepNhan, TiemChungChiDinhGoiDichVuTheoBenhNhanVo yeuCauVo);

        Task<List<LookupItemVo>> GetListTenGoiDichVu(List<long> yeuCauGoiDichVuIds);
        #endregion

        #region Cập nhật xử lý lưu nhập xuất chung 1 lần
        Task XuLySoLuongChiDinhVacxinAsyncVer2(ICollection<YeuCauDichVuKyThuat> yeuCauDichVuKyThuats);
        Task XuLyXoaSoLuongChiDinhVacxinAsyncVer2(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, List<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTiets);
        Task XuLyThemSoLuongChiDinhVacxinAsyncVer2(YeuCauDichVuKyThuat yeuCauDichVuKyThuat, List<NhapKhoDuocPhamChiTiet> nhapKhoDuocPhamChiTiets, CauHinhChung cauHinhChung, long khoVacXinId);
        #endregion
    }
}
