using Camino.Core.Helpers;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruThongTinHanhChinhViewModel : BaseViewModel
    {
        #region Tab hành chính
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay { get; set; }
        //public string NgaySinhDisplay => NgaySinh != null && ThangSinh != null && NamSinh != null ? DateTime.Parse($"{NgaySinh}/{ThangSinh}/{NamSinh}").ApplyFormatDate() : NamSinh?.ToString();
        public int? Tuoi => NamSinh != null ? DateTime.Now.Year - NamSinh : null;
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh?.GetDescription();
        public string DiaChi { get; set; }
        public long? QuocTichId { get; set; }
        public string QuocTichDisplay { get; set; }
        public long? DanTocId { get; set; }
        public string DanTocDisplay { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NgheNghiepDisplay { get; set; }
        public string NoiLamViec { get; set; }
        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeQuanHeNhanThanDisplay { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public bool? CoBHYT { get; set; }
        public string CoBHYTDisplay => CoBHYT == true ? $"BHYT ({BHYTMucHuong}%)" : "Viện phí";
        public string BHYTMaSoThe { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public string BHYTNgayHieuLucDisplay => BHYTNgayHieuLuc?.ApplyFormatDate();
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTNgayHetHanDisplay => BHYTNgayHetHan?.ApplyFormatDate();
        public int? BHYTMucHuong { get; set; }
        public string BHYTMucHuongDisplay => BHYTMucHuong != null ? $"{BHYTMucHuong}%" : "";
        public string TuyenKham { get; set; }
        public EnumNhomMau? NhomMau { get; set; }
        public string NhomMauDisplay => NhomMau?.ToString();
        public EnumYeuToRh? YeuToRh { get; set; }
        public string YeuToRhDisplay => YeuToRh?.GetDescription();
        #endregion

        #region Tab quản lý người bệnh
        public DateTime ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay => ThoiDiemTiepNhan.ApplyFormatDateTimeSACH();
        public long? NoiTiepNhanId { get; set; }
        public string NoiTiepNhanDisplay { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public string NoiGioiThieuDisplay { get; set; }
        public DateTime ThoiDiemNhapVien { get; set; }
        public string ThoiDiemNhapVienDisplay => ThoiDiemNhapVien.ApplyFormatDateTimeSACH();
        public DateTime? ThoiDiemRaVien { get; set; }
        public string ThoiDiemRaVienDisplay => ThoiDiemRaVien?.ApplyFormatDateTimeSACH();
        public int? SoLanVaoVienDoBenhNay { get; set; }
        public bool? CoChuyenVien { get; set; }
        public LoaiChuyenTuyen? LoaiChuyenTuyen { get; set; }
        public string LoaiChuyenTuyenDisplay => LoaiChuyenTuyen?.GetDescription();
        public long? ChuyenDenBenhVienId { get; set; }
        public string ChuyenDenBenhVienDisplay { get; set; }
        public EnumHinhThucRaVien? HinhThucRaVien { get; set; }
        public string HinhThucRaVienDisplay => HinhThucRaVien?.GetDescription();
        public DateTime? NgayTaiKham { get; set; }
        public string NgayTaiKhamDisplay => NgayTaiKham?.ApplyFormatDate();
        public bool IsTaiKham => NgayTaiKham != null ? true : false;
        public int? TongSoNgayDieuTri => ((ThoiDiemRaVien ?? DateTime.Now) - ThoiDiemNhapVien).Days + 1;
        //public int? TongSoNgayDieuTri => ThoiDiemRaVien != null ? (ThoiDiemRaVien.Value - ThoiDiemNhapVien).Days : (DateTime.Now - ThoiDiemNhapVien).Days;
        public bool? CoThuThuat { get; set; }
        public bool? CoPhauThuat { get; set; }
        #endregion
    }
}
