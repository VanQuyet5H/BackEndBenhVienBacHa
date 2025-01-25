using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class ChanDoanICD
    {
        public long KeyId { get; set; }
        public string Ma { get; set; }
        public string DisplayName => $"{Ma} - {TenTiengViet}";
        public string TenTiengViet { get; set; }
        public string TenTiengAnh { get; set; }
    }

    #region Bác sĩ điều trị
    public class EkipDieuTriGridVo : GridItem
    {
        public long NoiTruBenhAnId { get; set; }
        public long BacSiId { get; set; }
        public string BacSiDisplay { get; set; }
        public long DieuDuongId { get; set; }
        public string DieuDuongDisplay { get; set; }
        public long NhanVienLapId { get; set; }
        public string NhanVienLapDisplay { get; set; }
        public long KhoaPhongDieuTriId { get; set; }
        public string KhoaPhongDieuTriDisplay { get; set; }
        public DateTime TuNgay { get; set; }
        public string TuNgayDisplay => TuNgay.ApplyFormatDateTimeSACH();
        public DateTime? DenNgay { get; set; }
        public string DenNgayDisplay => DenNgay?.ApplyFormatDateTimeSACH();
        public bool IsFirstData { get; set; }
    }
    #endregion

    #region Cấp giường
    public class CapGiuongVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? GiuongBenhId { get; set; } //Update người nhà không cần giường
        public EnumLoaiGiuong? LoaiGiuong { get; set; }
        public bool BaoPhong { get; set; }
        public DateTime ThoiDiemBatDauSuDung { get; set; }
        public DateTime? ThoiDiemKetThucSuDung { get; set; }
        public long DichVuGiuongBenhVienId { get; set; }
        public DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public string GhiChu { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
    }

    public class CapGiuongGridVo : GridItem
    {
        public long PhongChiDinhId { get; set; }
        public string PhongChiDinhDisplay { get; set; }
        public long KhoaChiDinhId { get; set; }
        public string KhoaChiDinhDisplay { get; set; }
        public long? GiuongBenhId { get; set; }
        public string TenGiuong { get; set; }
        public EnumLoaiGiuong LoaiGiuong { get; set; }
        public string LoaiGiuongDisplay => LoaiGiuong.GetDescription();
        public long DichVuGiuongBenhVienId { get; set; }
        public string DichVuGiuongBenhVienDisplay { get; set; }
        public DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public string DoiTuongSuDungDisplay => DoiTuongSuDung?.GetDescription();
        public DateTime ThoiGianNhan { get; set; }
        public string ThoiGianNhanDisplay => ThoiGianNhan.ApplyFormatDateTimeSACH();
        public DateTime? ThoiGianTra { get; set; }
        public string ThoiGianTraDisplay => ThoiGianTra?.ApplyFormatDateTimeSACH();
        public bool BaoPhong { get; set; }
        public string GhiChu { get; set; }
        public bool? DaQuyetToan { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaBHYT { get; set; }
        public string MaDichVuGiuongBenhVien { get; set; }
        public decimal ThanhTienTamTinh { get; set; }
        public int LoaiGiaDichVuCoHieuLuc { get; set; }
        public bool IsFirstData { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class ChiTietSuDungGiuongTheoNgayVo
    {
        public ChiTietSuDungGiuongTheoNgayVo()
        {
            SuDungGiuongTheoNgays = new List<SuDungGiuongTheoNgayVo>();
        }

        public List<SuDungGiuongTheoNgayVo> SuDungGiuongTheoNgays { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class SuDungGiuongTheoNgayVo : GridItem
    {
        public SuDungGiuongTheoNgayVo()
        {
            ChiTietGiuongBenhVienChiPhis = new List<ChiTietGiuongBenhVienChiPhiVo>();
            //GiuongBenhVienChiPhis = new List<GiuongBenhVienChiPhiVo>();
        }
        public DateTime NgayPhatSinh { get; set; }
        public string NgayPhatSinhDisplay => NgayPhatSinh.ApplyFormatDate();
        public List<ChiTietGiuongBenhVienChiPhiVo> ChiTietGiuongBenhVienChiPhis { get; set; }
        //public List<GiuongBenhVienChiPhiVo> GiuongBenhVienChiPhis { get; set; }
    }

    public enum LoaiChiPhiGiuongBenh
    {
        [Description("Dịch vụ giường BV")]
        ChiPhiGiuongBenhVien = 1,
        [Description("Dịch vụ giường BHYT")]
        ChiPhiGiuongBHYT = 2,
    }

    //public class GiuongBenhVienChiPhiVo : GridItem
    //{
    //    public GiuongBenhVienChiPhiVo()
    //    {
    //        ChiTietGiuongBenhVienChiPhis = new List<ChiTietGiuongBenhVienChiPhiVo>();
    //    }
    //    public LoaiChiPhiGiuongBenh LoaiChiPhiGiuongBenh { get; set; }
    //    public string Loai => LoaiChiPhiGiuongBenh.GetDescription();
    //    public List<ChiTietGiuongBenhVienChiPhiVo> ChiTietGiuongBenhVienChiPhis { get; set; }
    //}

    public class ChiTietGiuongBenhVienChiPhiVo : GridItem
    {
        public ChiTietGiuongBenhVienChiPhiVo()
        {
            ChiTietGiuongBenhVienChiPhiBHYTs = new List<ChiTietGiuongBenhVienChiPhiVo>();
            isCreated = false;
        }

        public LoaiChiPhiGiuongBenh LoaiChiPhiGiuongBenh { get; set; }
        public long KhoaChiDinhId { get; set; }
        public string KhoaChiDinhDisplay { get; set; }
        public long? GiuongChuyenDenId { get; set; }
        public string GiuongChuyenDenDisplay { get; set; }
        public EnumLoaiGiuong LoaiGiuong { get; set; }
        public string LoaiGiuongDisplay => LoaiGiuong.GetDescription();
        public bool? BaoPhong { get; set; }
        public long DichVuGiuongId { get; set; }
        public string DichVuGiuongDisplay { get; set; }
        public DoiTuongSuDung? DoiTuong { get; set; }
        public string DoiTuongDisplay => DoiTuong?.GetDescription();
        public double SoLuong { get; set; }
        public int SoLuongGhep { get; set; }
        public decimal DonGia { get; set; }
        public decimal? SoTienBaoHiem { get; set; }
        public decimal ThanhTien { get; set; }
        public bool isCreated { get; set; }
        public long? ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public List<ChiTietGiuongBenhVienChiPhiVo> ChiTietGiuongBenhVienChiPhiBHYTs { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }
        public bool CoDichVuNayTrongGoi { get; set; }
        public bool LaDichVuTrongGoi { get; set; } 
        public bool CoDichVuNayTrongGoiKhuyenMai { get; set; }
        public bool CoThongTinMienGiam { get; set; }
        public bool LaDichVuKhuyenMai { get; set; }
    }

    public class GiuongSuDungTheoNgaySearch
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime NgayPhatSinh { get; set; }
    }

    public class ThoiDiemNhanGiuongVo
    {
        public DateTime ThoiDiemNhanGiuong { get; set; }
        public DateTime MinThoiDiemNhanGiuong { get; set; }
    }

    public class ThongTinGiaDichVuGiuongVo
    {
        public decimal DonGia { get; set; }
        public decimal ThanhTien => DonGia;
        public decimal? DonGiaBHYT { get; set; }
        public decimal? ThanhTienBHYT => DonGiaBHYT;
    }
    #endregion

    #region Chuyển khoa
    public class ChuyenKhoaGridVo : GridItem
    {
        public long NoiTruBenhAnId { get; set; }
        public long? KhoaPhongChuyenDiId { get; set; }
        public string KhoaPhongChuyenDiDisplay { get; set; }
        public long KhoaPhongChuyenDenId { get; set; }
        public string KhoaPhongChuyenDenDisplay { get; set; }
        public DateTime ThoiDiemVaoKhoa { get; set; }
        public string ThoiDiemVaoKhoaDisplay => ThoiDiemVaoKhoa.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemRaKhoa { get; set; }
        public string ThoiDiemRaKhoaDisplay => ThoiDiemRaKhoa?.ApplyFormatDateTimeSACH();
        public long? ChanDoanVaoKhoaICDId { get; set; }
        public string ChanDoanVaoKhoaICDDisplay { get; set; }
        public string ChanDoanVaoKhoaGhiChu { get; set; }
        public string LyDoChuyenKhoa { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public string NhanVienChiDinhDisplay { get; set; }
        public List<long> BacSiDieuTriId { get; set; }
        public string BacSiDieuTriDisplay { get; set; }
        public bool IsFirstData { get; set; }
    }

    public class KhoaPhongChuyenDen
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
    }
    #endregion
}