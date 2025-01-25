using System;
using System.Collections.Generic;
using System.Linq;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XacNhanBHYTs
{
    public class DanhSachChoGridVo : GridItem
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string SearchString { get; set; }

        public string MaTN { get; set; }

        public string MaBN { get; set; }


        public string HoTen { get; set; }

        public string NamSinhDisplay { get; set; }

        public string TenGioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string SoDienThoai { get; set; }

        public string SoDienThoaiFormat { get; set; }

        public decimal? SoTienDaXacNhan =>
            (DanhSachDichVuDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.YeuCauTruyenMauDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.DichVuGiuongDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.YeuCauTruyenMauDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuGiuongDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0);

        public decimal? SoTienChoXacNhan =>
            (DanhSachDichVuDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.YeuCauTruyenMauDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.DichVuGiuongDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKhamBenhDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuKyThuatDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.YeuCauTruyenMauDuocHuongBhyt?.Select(o => o.SoTienDaXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.DichVuGiuongDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.DuocPhamDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.VatTuBenhVienDuocHuongBhyt?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0) +
            (DanhSachDichVuNgoaiTruDuocHuongBHYT?.ToaThuocDuocHuongBHYT?.Select(o => o.SoTienChoXacNhan).DefaultIfEmpty(0).Sum() ?? 0);

        public DateTime ThoiDiemDuyet { get; set; }

        public bool ChuaXacNhan { get; set; }

        public DateTime? ThoiDiemTiepNhan { get; set; }

        public string ThoiDiemTiepNhanDisplay => ThoiDiemTiepNhan != null ? ThoiDiemTiepNhan.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        public DanhSachDichVuDuocHuongBHYTVo DanhSachDichVuDuocHuongBHYT { get; set; }
        public DanhSachDichVuDuocHuongBHYTVo DanhSachDichVuNgoaiTruDuocHuongBHYT { get; set; }

        public long? YeuCauTiepNhanNgoaiTruCanQuyetToanId { get; set; }

        public string HuongXuLy { get; set; }

        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
    }

    public class DanhSachDichVuDuocHuongBHYTVo
    {
        public long YeuCauTiepNhanId { get; set; }
        public IEnumerable<DichVuDuocHuongBHYTVo> DichVuKhamBenhDuocHuongBHYT { get; set; }
        public IEnumerable<DichVuDuocHuongBHYTVo> DichVuKyThuatDuocHuongBHYT { get; set; }
        public IEnumerable<DichVuDuocHuongBHYTVo> DichVuGiuongDuocHuongBHYT { get; set; }
        public IEnumerable<DichVuDuocHuongBHYTVo> DuocPhamDuocHuongBHYT { get; set; }
        public IEnumerable<DichVuDuocHuongBHYTVo> ToaThuocDuocHuongBHYT { get; set; }
        public IEnumerable<DichVuDuocHuongBHYTVo> VatTuBenhVienDuocHuongBhyt { get; set; }
        public IEnumerable<DichVuDuocHuongBHYTVo> YeuCauTruyenMauDuocHuongBhyt { get; set; }
    }
    public class DichVuDuocHuongBHYTVo
    {
        public double Soluong { get; set; }
        public bool? BaoHiemChiTra { get; set; }

        public int PhanTramCuThe => (TiLeDv * MucHuong) / 100;

        public int TiLeDv { get; set; }

        public int MucHuong { get; set; }

        public decimal DgThamKhao { get; set; }

        public decimal DonGiaBHYTThanhToan => (PhanTramCuThe * DgThamKhao) / 100;

        public decimal SoTienDaXacNhan => BaoHiemChiTra == true ? (decimal)(Soluong * (double)DonGiaBHYTThanhToan) : 0;
        public decimal SoTienChoXacNhan => BaoHiemChiTra == null ? (decimal)(Soluong * (double)DonGiaBHYTThanhToan) : 0;
        public string CachGiaiQuyet { get; set; }
    }

    public class LichSuXacNhanBHYTGridVo : GridItem
    {
        public string MaTN { get; set; }

        public string MaBN { get; set; }

        public long? IdBenhNhan { get; set; }

        public string HoTen { get; set; }

        public string NamSinh { get; set; }

        public string TenGioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string SoDienThoai { get; set; }

        public string SoDienThoaiDisplay { get; set; }

        public decimal? SoTienDaXacNhan =>
            ChiTietLichSuXacNhanBHYTs?.Sum(o => (decimal)o.Soluong * o.DonGiaBHYTThanhToan);

        public double SoLuong { get; set; }

        public string ThoiDiemDuyetBaoHiem => ThoiDiemDuyet.ApplyFormatDateTimeSACH();

        public DateTime ThoiDiemDuyet { get; set; }

        public string NhanVienDuyetBaoHiem { get; set; }

        public IEnumerable<ChiTietLichSuXacNhanBHYTVo> ChiTietLichSuXacNhanBHYTs { get; set; }
    }

    public class ChiTietLichSuXacNhanBHYTVo
    {
        public double Soluong { get; set; }
        public decimal DonGiaBHYTThanhToan => (decimal)PhanTramCuThe * DgThamKhao.GetValueOrDefault() / 100;

        public int TiLeDv { get; set; }

        public int MucHuong { get; set; }

        public decimal? DgThamKhao { get; set; }

        public double PhanTramCuThe => (double)(TiLeDv * MucHuong) / 100;
    }

    public class LichSuXacNhanBHYTAdditionalSearch : LichSuXacNhanBHYTGridVo
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string SearchString { get; set; }
    }

    public class TimKiemThongTinBenhNhan
    {
        public string TimKiemMaBNVaMaTN { get; set; }
    }
}