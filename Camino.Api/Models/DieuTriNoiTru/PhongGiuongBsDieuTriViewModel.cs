using Camino.Core.Domain.ValueObject.DieuTriNoiTru;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class EkipDieuTriViewModel : BaseViewModel
    {
        public long? NoiTruBenhAnId { get; set; }
        public long? BacSiId { get; set; }
        public long? DieuDuongId { get; set; }
        public long? NhanVienLapId { get; set; }
        public long? KhoaPhongDieuTriId { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class CapGiuongViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? GiuongBenhId { get; set; }
        public EnumLoaiGiuong? LoaiGiuong { get; set; }
        public bool BaoPhong { get; set; }
        public DateTime? ThoiDiemBatDauSuDung { get; set; }
        public DateTime? ThoiDiemKetThucSuDung { get; set; }
        public long? DichVuGiuongBenhVienId { get; set; }
        public DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public string GhiChu { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
    }

    #region Chi tiết sử dụng giường
    public class ChiTietSuDungGiuongTheoNgayViewModel
    {
        public ChiTietSuDungGiuongTheoNgayViewModel()
        {
            SuDungGiuongTheoNgays = new List<SuDungGiuongTheoNgayViewModel>();
        }

        public List<SuDungGiuongTheoNgayViewModel> SuDungGiuongTheoNgays { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class SuDungGiuongTheoNgayViewModel : BaseViewModel
    {
        public SuDungGiuongTheoNgayViewModel()
        {
            ChiTietGiuongBenhVienChiPhis = new List<ChiTietGiuongBenhVienChiPhiViewModel>();
        }

        public DateTime? NgayPhatSinh { get; set; }
        public string NgayPhatSinhDisplay => NgayPhatSinh?.ApplyFormatDate();
        public List<ChiTietGiuongBenhVienChiPhiViewModel> ChiTietGiuongBenhVienChiPhis { get; set; }
        //public List<GiuongBenhVienChiPhiViewModel> GiuongBenhVienChiPhis { get; set; }
    }

    //public class GiuongBenhVienChiPhiViewModel : BaseViewModel
    //{
    //    public GiuongBenhVienChiPhiViewModel()
    //    {
    //        ChiTietGiuongBenhVienChiPhis = new List<ChiTietGiuongBenhVienChiPhiViewModel>();
    //    }
    //    public LoaiChiPhiGiuongBenh? LoaiChiPhiGiuongBenh { get; set; }
    //    public string Loai => LoaiChiPhiGiuongBenh?.GetDescription();
    //    public List<ChiTietGiuongBenhVienChiPhiViewModel> ChiTietGiuongBenhVienChiPhis { get; set; }
    //}

    public class ChiTietGiuongBenhVienChiPhiViewModel : BaseViewModel
    {
        public ChiTietGiuongBenhVienChiPhiViewModel()
        {
            ChiTietGiuongBenhVienChiPhiBHYTs = new List<ChiTietGiuongBenhVienChiPhiViewModel>();
        }

        public LoaiChiPhiGiuongBenh? LoaiChiPhiGiuongBenh { get; set; }
        public long? KhoaChiDinhId { get; set; }
        public string KhoaChiDinhDisplay { get; set; }
        public long? GiuongChuyenDenId { get; set; }
        public string GiuongChuyenDenDisplay { get; set; }
        public EnumLoaiGiuong? LoaiGiuong { get; set; }
        public string LoaiGiuongDisplay => LoaiGiuong.GetDescription();
        public bool? BaoPhong { get; set; }
        public long? DichVuGiuongId { get; set; }
        public string DichVuGiuongDisplay { get; set; }
        public DoiTuongSuDung? DoiTuong { get; set; }
        public string DoiTuongDisplay => DoiTuong?.GetDescription();
        public double? SoLuong { get; set; }
        public int? SoLuongGhep { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? SoTienBaoHiem { get; set; }
        public decimal? ThanhTien { get; set; }
        public bool isCreated { get; set; }
        public long? ThanhToanTheoYeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public List<ChiTietGiuongBenhVienChiPhiViewModel> ChiTietGiuongBenhVienChiPhiBHYTs { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public bool CoDichVuNayTrongGoi { get; set; }
        public bool LaDichVuTrongGoi { get; set; }
        public bool CoDichVuNayTrongGoiKhuyenMai { get; set; }
        public bool CoThongTinMienGiam { get; set; }
        public bool LaDichVuKhuyenMai { get; set; }
    }
    #endregion

    public class KhoaPhongDieuTriViewModel : BaseViewModel
    {
        public long? NoiTruBenhAnId { get; set; }
        public long? KhoaPhongChuyenDiId { get; set; }
        public string KhoaPhongChuyenDiDisplay { get; set; }
        public long? KhoaPhongChuyenDenId { get; set; }
        public string KhoaPhongChuyenDenDisplay { get; set; }
        public DateTime? ThoiDiemVaoKhoa { get; set; }
        public DateTime? ThoiDiemRaKhoa { get; set; }
        public long? ChanDoanVaoKhoaICDId { get; set; }
        public string ChanDoanVaoKhoaICDDisplay { get; set; }
        public string ChanDoanVaoKhoaGhiChu { get; set; }
        public string LyDoChuyenKhoa { get; set; }
        public long? NhanVienChiDinhId { get; set; }
    }

    public class KiemTraDichVuCapGiuongViewModel
    {
        public DateTime? ThoiGianNhan { get; set; }
        public DateTime? ThoiGianTra { get; set; }
        public long? DichVuGiuongId { get; set; }
        public long? GiuongBenhId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
    }

    public class ThongTinGetGiaDichVuGiuongViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long DichVuGiuongId { get; set; }
        public DateTime NgayPhatSinh { get; set; }
        public bool? BaoPhong { get; set; }
    }
}