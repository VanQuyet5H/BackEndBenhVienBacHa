using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Camino.Core.Domain;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;

namespace Camino.Services.YeuCauKhamBenh
{
    public partial interface IYeuCauKhamBenhService
    {
        #region Hàng chờ
        //// grid

        Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachBenhNhanChoKham(long phongKhamHienTaiId, string searchString = null, bool laKhamDoan = false);
        Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachChoKhamHangDoiChungAsync(long phongKhamHienTaiId, string searchString = null, bool laKhamDoan = false);
        Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachChoKetLuanHangDoiChungAsync(long phongKhamHienTaiId, string searchString = null);
        Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachLamChiDinhHienTaiAsync(long phongKhamHienTaiId, string searchString = null);
        Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachDoiKetLuanHienTaiAsync(long phongKhamHienTaiId, string searchString = null, bool laKhamDoan = false);

        Task<GridDataSource> GetDataForGridBenhNhanLamChiDinhAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGriBenhNhanLamChiDinhdAsync(QueryInfo queryInfo);

        // get data
        Task<SoLuongYeuCauHienTaiVo> GetSoLuongYeuCauHienTai(long phongKhamId, bool laKhamDoan);
        Task<PhongBenhVienHangDoi> TimKiemBenhNhanTrongHangDoi(string searchString, long phongKhamId);
        Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamTheoPhongKham(long phongKhamId, long? hangDoiId, bool laKhamDoan = false, Enums.EnumTrangThaiHangDoi trangThai = Enums.EnumTrangThaiHangDoi.DangKham);
        Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhTiepTheoTheoPhongKham(long hangDoiId, bool laKhamDoan = false);
        Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamTheoPhongKhamLuuTabKhamBenh(long phongKhamId, long? hangDoiId, bool laKhamDoan = false, Enums.EnumTrangThaiHangDoi trangThai = Enums.EnumTrangThaiHangDoi.DangKham);
        Task<List<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>> GetTemplateCacDichVuKhamSucKhoeAsync(long yeuCauTiepNhanId, long yeuCauKhamBenhId);
        Task<IQueryable<Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh>> GetTemplateCacDichVuKhamSucKhoeAsyncVer2(long yeuCauTiepNhanId, long yeuCauKhamBenhId);
        // xử lý

        List<string> InPhieuKhamBenh(PhieuKhamBenhVo phieuKhamBenhVo);

        string InGiayChuyenVien(long yeuCauKhamBenhId);
        string XemGiayNghiHuongBHYTLien1(ThongTinNgayNghiHuongBHYT thongTinNgayNghi);
        string XemGiayNghiHuongBHYTLien2(ThongTinNgayNghiHuongBHYT thongTinNgayNghi);
        Task<bool> KiemTraNgayTiepNhan(DateTime? ngayTiepNhan, DateTime? ngayNghiHuong, long yeuCauKhamBenhId);
        Task<bool> KiemTraDenNgay(DateTime? ngayTiepNhan, DateTime? ngayNghiHuong, long yeuCauKhamBenhId);
        #endregion

        #region Xử lý

        Task XuLyCapNhatBenhNhanVangAsync(long hangDoiId, long phongBenhVienId);
        Task XuLyHoanThanhCongDoanKhamHienTaiCuaBenhNhan(long hangDoiHienTaiId, bool hoanThanhKham = false);
        Task<bool> KiemTraCoBenhNhanKhacDangKhamTrongPhong(long? hangDoiDangKham, long phongBenhVienId, bool laKhamDoan = false);
        Task XuLyDataYeuCauKhamBenhChuyenKhamAsync(Core.Domain.Entities.YeuCauKhamBenhs.YeuCauKhamBenh yeuCauKham, bool coBaoHiem);

        Task KiemTraDataChuyenKhamAsync(YeuCauTiepNhan yeuCauTiepNhan, long yeuCauKhamBenhId, long dichVuKhamBenhId);

        #endregion

        #region  Khám đoàn khám bệnh tất cả phòng
        Task<ICollection<BenhNhanChoKhamGridVo>> GetDanhSachHangDoiKhamDoanTatCaAsync(KhamDoanKhamBenhTatCaPhongTimKiemVo timKiemVo);
        Task<GridDataSource> GetDataForGridHangDoiKhamDoanTatCaAsync(QueryInfo queryInfo);
        Task<GridDataSource> GetTotalPageForGriHangDoiKhamDoanTatCaAsync(QueryInfo queryInfo);
        Task<PhongBenhVienHangDoi> GetYeuCauKhamBenhDangKhamTheoHopDongKhamDoanAsync(long hangDoiId);
        Task<List<KetQuaMauDichVuKyThuatDataVo>> GetKetQuaMauDichVuKyThuatAsync(KhamDoanTatCaPhongKetQuaMauVo ketQuaMuaVo);
        Task<KhamDoanTatCaPhongDichVuChuaThucHienVo> KiemTraDichVuKhamDoanChuaThucHienAsync(KhamDoanTatCaPhongKiemTraDichVuChuaThucHienVo kiemTraVo);
        #endregion

        #region cập nhật quay lại chưa khám
        Task XuLyQuayLaiChuaKhamAsync(long hangDoiId);
        Task XuLyQuayLaiChuaKhamKhamDoanTheoPhongAsync(PhongBenhVienHangDoi hangDoi, string dataDefaultChuyenKhoaKham = null);
        #endregion

        #region BVHD-3574
        Task<bool> KiemTraKhoaHienTaiCoNhieuNguoiBenhAsync(long? phongHienTaiId = null);

        Task<bool> KiemTraDichVuHienTaiCoNhieuNguoiBenhAsync(long dichVuBenhVienId);
        #endregion


        #region BVHD-3817

        void KTNgayGiayNghiHuongBHYT(ThongTinNgayNghiHuongBHYT thongTinNgayNghi);
        ThongTinNgayNghiHuongBHYTTiepNhan GetThongTinNgayNghiHuongBHYT(long yeuCauKhamBenhId);

        #endregion


        #region BVHD-3797
        Task<List<string>> KiemTraCoDichVuChuaThucHienAsync(long yeuCauKhamBenhId);

        #endregion

        #region BVHD-3895

        Task<bool> KiemTraDichVuKhamHienThiTenVietTatAsync(long dichVuKhamBenhId);

        #endregion

        #region Cập nhật 01/12/2022
        (bool, bool) KiemTraCoGoiVaKhuyenMaiTheoNguoiBenhId(long nguoiBenhId);
        YeuCauDichVuKyThuat GetDichVuKyThuatDieuTriNgoaiTruTheoYeuCauKhamBenhId(long yeuCauKhamBenhId);
        List<LookupItemVo> GetListTenICD(List<long> id);
        #endregion
    }
}
