using System;
using Camino.Api.Models.BenhNhans;
using Camino.Api.Models.DoiTuongUuDais;
using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.Domain;

namespace Camino.Api.Models.YeuCauTiepNhan
{
    public class YeuCauTiepNhanViewModel : BaseViewModel
    {
        public bool? CoBHYT { get; set; }
        public bool? CoBHTN { get; set; }
        public bool? DuocUuDai { get; set; }
        public long? BenhNhanId { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoChungMinhThu { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public Enums.EnumNhomMau? NhomMau { get; set; }
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
        public string BHYTngayHieuLucStr { get; set; }
        public string BHYTngayHetHanStr { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public DateTime? BHYTNgayHetHan { get; set; }
        public string BHYTDiaChi { get; set; }
        public string BHYTCoQuanBhXH { get; set; }
        public DateTime? BHYTNgayDu5Nam { get; set; }
        public bool? BHYTDuocMienCungChiTra { get; set; }
        public DateTime? BHYTNgayDuocMienCungChiTra { get; set; }
        public string BHYTMaKhuVuc { get; set; }
        public long? BHYTGiayMienCungChiTraId { get; set; }
        public string GiayMienCungChiTraDisplay { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public Enums.EnumYeuCauTiepNhan? LyDoTiepNhan { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public long? NhanVienTiepNhanId { get; set; }
        public long? NoiTiepNhanId { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }

        public string LyDoVaoVienDisplay { get; set; }

        public bool? LaTaiKham { get; set; }
        public string TrieuChungTiepNhan { get; set; }
        public int? LoaiTaiNan { get; set; }

        public bool? DuocChuyenVien { get; set; }
        public long? GiayChuyenVienId { get; set; }

        public string GiayChuyenVienDisplay { get; set; }

        public DateTime? ThoiGianChuyenVien { get; set; }
        public long? NoiChuyenId { get; set; }
        public long? DoiTuongUuTienKhamChuaBenhId { get; set; }

        public int? KetQuaDieuTri { get; set; }
        public int? TinhTrangRaVien { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiYeuCauTiepNhan { get; set; }
        public DateTime ThoiDiemCapNhatTrangThai { get; set; }
        public int? TinhTrangThe { get; set; }
        public bool? IsCheckedBHYT { get; set; }
        public string BHYTMaDKBD { get; set; }

        public string DKBD { get; set; }

        public long? NoiGioiThieuId { get; set; }
        public long? HinhThucDenId { get; set; }

        public long? DoiTuongUuDaiId { get; set; }
        public long? CongTyUuDaiId { get; set; }
        public string SoChuyenTuyen { get; set; }
        public Enums.TuyenChuyenMonKyThuat? TuyenChuyen { get; set; }
        public string LyDoChuyen { get; set; }
        public int? BHYTMucHuong { get; set; }

        public DoiTuongUuDaiViewModel DoiTuongUuDai { get; set; }

        public CongTyUuDaiViewModel CongTyUuDai { get; set; }

        public GiayChuyenVienBenhNhanViewModel GiayChuyenVien { get; set; }

        public BenhNhanViewModel BenhNhan { get; set; }

        public string NhanVienDuyet { get; set; }

        public string ThoiDiemDuyet { get; set; }
        public string NoiChuyenDi { get; set; }

        //BVHD-3941
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }
}
