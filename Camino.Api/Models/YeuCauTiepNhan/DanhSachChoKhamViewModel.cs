using Camino.Api.Models.PhongBenhVien;
using Camino.Core.Domain;
using System;

namespace Camino.Api.Models.YeuCauTiepNhan
{
    public class DanhSachChoKhamViewModel : BaseViewModel
    {
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public int? NhomMau { get; set; }
        public long? NgheNghiepId { get; set; }
        public string NoiLamViec { get; set; }
        public long? QuocTichId { get; set; }
        public long? DanTocId { get; set; }
        public string DiaChi { get; set; }
        public long? PhuongXaId { get; set; }
        public long? QuanHuyenId { get; set; }
        public long? TinhThanhId { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string NguoiLienHeHoTen { get; set; }
        public long? NguoiLienHeQuanHeNhanThanId { get; set; }
        public string NguoiLienHeSoDienThoai { get; set; }
        public string NguoiLienHeEmail { get; set; }
        public string NguoiLienHeDiaChi { get; set; }
        public long? NguoiLienHePhuongXaId { get; set; }
        public long? NguoiLienHeQuanHuyenId { get; set; }
        public long? NguoiLienHeTinhThanhId { get; set; }
        public string BHYTMaSoThe { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTdiaChi { get; set; }
        public string BHYTcoQuanBhxh { get; set; }
        public DateTime? BHYTngayDu5Nam { get; set; }
        public DateTime? BHYTngayDuocMienCungChiTra { get; set; }
        public string BHYTmaKhuVuc { get; set; }
        public long? BHYTgiayMienCungChiTraId { get; set; }
        public long? BHTNcongTyBaoHiemId { get; set; }
        public string BHTNmaSoThe { get; set; }
        public string BHTNdiaChi { get; set; }
        public string BHTNsoDienThoai { get; set; }
        public DateTime? BHTNngayHieuLuc { get; set; }
        public DateTime? BHTNngayHetHan { get; set; }
        public int LoaiYeuCauTiepNhan { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public long? NhanVienTiepNhanId { get; set; }
        public long? NoiTiepNhanId { get; set; }
        public int? LyDoVaoVien { get; set; }
        public bool? LaTaiKham { get; set; }
        public string TrieuChungTiepNhan { get; set; }
        public int? LoaiTaiNan { get; set; }
        public bool? DuocChuyenVien { get; set; }
        public long? GiayChuyenVienId { get; set; }
        public DateTime? ThoiGianChuyenVien { get; set; }
        public long? NoiChuyenId { get; set; }
        public long? DoiTuongUuTienKhamChuaBenhId { get; set; }
        public int? KetQuaDieuTri { get; set; }
        public int? TinhTrangRaVien { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh? TrangThaiYeuCauTiepNhan { get; set; }
        public DateTime? ThoiDiemCapNhatTrangThai { get; set; }
        public int? TinhTrangThe { get; set; }
        public bool? IsCheckedBHYT { get; set; }
        public string BHYTMaDKBD { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public long? HinhThucDenId { get; set; }
        public long? DoiTuongUuDaiId { get; set; }
        public long? CongTyUuDaiId { get; set; }
        public string SoChuyenTuyen { get; set; }
        public int? TuyenChuyen { get; set; }
        public string LyDoChuyen { get; set; }
        public virtual PhongBenhVienViewModel NoiTiepNhan { get; set; }
    }   
}
