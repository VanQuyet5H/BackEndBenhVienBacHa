using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class YeuCauTiepNhanGridVo : GridItem
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
        public string NhanVienTiepNhan { get; set; }
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
        public long? KhoaPhongId { get; set; }
        public string Khoa { get; set; }
        public string Phong { get; set; }
        public string BacSi { get; set; }
        public string TenBN { get; set; }
        public string DoiTuong { get; set; }
        public string ThoiDiemTiepNhanTu { get; set; }
        public string ThoiDiemTiepNhanDen { get; set; }
        public long? DoiTuongBHYT { get; set; }
        public string TrangThaiYeuCauTiepNhanDisplay { get; set; }
        public string ThoiDiemTiepNhanDisplay { get; set; }
    }
    public class YeuCauTiepNhanSearch : GridItem
    {
        public long? PhongKhamId { get; set; }
        public string SearchChing { get; set; }
    }

    public class GridLichSuKCB : GridItem
    {
        public long? STT { get; set; }
        public string MaTheBHYT { get; set; }
        public string HoVaTen { get; set; }
        public string NgayVaoVien { get; set; }
        public string NgayRaVien { get; set; }
        public string CoSoKCB { get; set; }
        public string MaCoSoKCB { get; set; }
        public string KetQuaDieuTri { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTriNumber { get; set; }
        public string LyDoVaoVien { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVienNumber { get; set; }
        public string TinhTrangRaVien { get; set; }
        public Enums.EnumTinhTrangRaVien? TinhTrangRaVienNumber { get; set; }
        public DateTime? NgayVaoDateTime { get; set; }
        public DateTime? NgayRaDateTime { get; set; }
    }

    public class GridLichSuKiemTraTheBHYT : GridItem
    {
        public long? STT { get; set; }
        public string UserKiemTra { get; set; }
        public string TenCSKCB { get; set; }
        public string MaCSKCB { get; set; }
        public string ThoiGianKiemTra { get; set; }
        public string NoiDungThongBao { get; set; }
        public DateTime? thoiGianKTDateTime { get; set; }

    }
}
