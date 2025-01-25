using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;
namespace Camino.Core.Domain.ValueObject.GoiBaoHiemYTe
{
    public class GoiDanhSachThongTinBenhNhanCoBHYT
    {
        public List<long> YeuCauTiepNhanIds { get; set; }
        public int? Version { get; set; }
    }
    public class ThongTinBenhNhanGoiBHYT 
    {
        public ThongTinBenhNhanGoiBHYT()
        {
            HoSoChiTietThuoc = new List<HoSoChiTietThuocVo>();
            HoSoChiTietDienBienBenh = new List<HoSoChiTietDienBienBenhVo>();
            HoSoCanLamSang = new List<HoSoCanLamSangVo>();
            HoSoChiTietDVKT = new List<HoSoChiTietDVKTVo>();
        }

        public string MaLienKet { get; set; }
        public int?  STT { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string NgaySinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string MaThe { get; set; }
        public string MaCoSoKCBBanDau { get; set; }
        public string GiaTriTheTu { get; set; }
        public string GiaTriTheDen { get; set; }
        public string TenBenh { get; set; }
        public string MaBenh { get; set; }
        public string MienCungChiTra { get; set; }
        public string MaBenhKhac { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        public string MaNoiChuyen { get; set; }
        public Enums.EnumMaTaiNan? MaTaiNan { get; set; }    
        public DateTime? NgayVaoTime { get; set; }
        public string NgayVao => NgayVaoTime?.ApplyFormatDate();
        public string NgayVaoStr { get; set; }
        public DateTime? NgayRaTime { get; set; }
        public string NgayRa => NgayRaTime?.ApplyFormatDate();
        public string NgayRaStr { get; set; }
        public double SoNgayDieuTri { get; set; }
        public Enums.EnumKetQuaDieuTri? KetQuaDieuTri { get; set; }
        public Enums.EnumTinhTrangRaVien? TinhTrangRaVien { get; set; }       
        public DateTime? NgayThanhToanTime { get; set; }
        public string NgayThanhToan => NgayThanhToanTime?.ApplyFormatDate();
        public double? TienThuoc { get; set; }
        public double? TienVatTuYTe { get; set; }
        public double? TienTongChi { get; set; }
        public double? TienBenhNhanThanhToan { get; set; }
        public double? TienBaoHiemThanhToan { get; set; }
        public double? TienNguonKhac { get; set; }
        public double? TienNgoaiDanhSach { get; set; }
        public double? TienBenhNhanCungChiTra { get; set; }
        public int? NamQuyetToan { get; set; }
        public int? ThangQuyetToan { get; set; }
        public Enums.EnumMaHoaHinhThucKCB? MaLoaiKCB { get; set; }
        public string MaKhoa { get; set; }
        public string MaCSKCB { get; set; }
        public string MaKhuVuc { get; set; }
        public string MaPhauThuatQuocTe { get; set; }
        public double? CanNang { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string DataJson { get; set; }
        public int? MucHuong { get; set; }
        public bool? IsDownLoad { get; set; }
        public int Version { get; set; }

        public List<HoSoChiTietThuocVo> HoSoChiTietThuoc { get; set; }
        public List<HoSoChiTietDienBienBenhVo> HoSoChiTietDienBienBenh { get; set; }
        public List<HoSoCanLamSangVo> HoSoCanLamSang { get; set; }
        public List<HoSoChiTietDVKTVo> HoSoChiTietDVKT { get; set; }
    }
    public class HoSoChiTietThuocVo
    {
        public string MaLienKet { get; set; }
        public int STT { get; set; }
        public string MaThuoc { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi? MaNhom { get; set; }
        public string MaNhomText => MaNhom.GetDescription();
        public string TenThuoc { get; set; }
        public string DonViTinh { get; set; }
        public string HamLuong { get; set; }
        public string DuongDung { get; set; }
        public string LieuDung { get; set; }
        public string SoDangKy { get; set; }
        public double SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public int? TyLeThanhToan { get; set; }
        public decimal? ThanhTien => (decimal)SoLuong * DonGia.GetValueOrDefault();
        public string MaKhoa { get; set; }
        public string MaBacSi { get; set; }
        public string MaBenh { get; set; }
        public string NgayYLenh { get; set; }
        public DateTime? NgayYLenhTime { get; set; }
        public string ThongTinThau { get; set; }
        public int? PhamVi { get; set; }
        public int? MucHuong { get; set; }
        public decimal? TienBenhNhanTuTra { get; set; }
        public decimal? TienNguonKhac { get; set; }
        public decimal? TienBaoHiemTuTra { get; set; }
        public decimal? TienBenhNhanTuChiTra { get; set; }
        public decimal? TienNgoaiDanhSach { get; set; }
      
        public Enums.EnumMaPhuongThucThanhToan? MaPhuongThucThanhToan { get; set; }
        public string MaPhuongThucThanhToanText => MaPhuongThucThanhToan.GetDescription();

    }
    public class HoSoChiTietDienBienBenhVo
    {
        public string MaLienKet { get; set; }
        public int STT { get; set; }
        public string DienBien { get; set; }
        public string HoiChuan { get; set; }
        public string PhauThuat { get; set; }
        public string NgayYLenh { get; set; }
        public DateTime? NgayYLenhTime { get; set; }
    }
    public class HoSoCanLamSangVo 
    {
        public string MaLienKet { get; set; }
        public int STT { get; set; }
        public string MaDichVu { get; set; }
        public string MaChiSo { get; set; }
        public string TenChiSo { get; set; }
        public string GiaTri { get; set; }
        public string MaMayXetNghiem { get; set; }
        public string MaMay { get; set; }
        public string MoTa { get; set; }
        public string KetLuan { get; set; }
        public string NgayKQ { get; set; }
        public DateTime? NgayKQTime { get; set; }
    }
    public class HoSoChiTietDVKTVo 
    {
        public string MaLienKet { get; set; }
        public int STT { get; set; }
        public string MaDichVu { get; set; }
        public string MaVatTu { get; set; }
        public string GoiVatTuYTe { get; set; }
        public string TenVatTu { get; set; }
        public string ThongTinThau { get; set; }
        public int? PhamVi { get; set; }
        public double? MucThanhToanToiDa { get; set; }
        public int? MucHuong { get; set; }
        public double? TienNguonKhac { get; set; }
        public double? TienBenhNhanThanhToan { get; set; }
        public double? TienBaoHiemThanhToan { get; set; }
        public double? TienBenhNhanCungChiTra { get; set; }
        public double? TienNgoaiDanhSach { get; set; }
        public string MaGiuong { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi? MaNhom { get; set; }
        public string MaNhomText => MaNhom.GetDescription();
        public string TenDichVu { get; set; }
        public string DonViTinh { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public int? TyLeThanhToan { get; set; }
        public decimal? ThanhTien => (decimal)SoLuong * DonGia.GetValueOrDefault();
        public string MaKhoa { get; set; }
        public string MaBacSi { get; set; }
        public string MaBenh { get; set; }
        public string NgayYLenh { get; set; }
        public DateTime? NgayYLenhTime { get; set; }
        public DateTime? NgayKetQuaTime { get; set; }
        public string NgayKetQua => NgayKetQuaTime?.ApplyFormatDate();

        public Enums.EnumMaPhuongThucThanhToan? MaPhuongThucThanhToan { get; set; }
        public string MaPhuongThucThanhToanText => MaPhuongThucThanhToan.GetDescription();
    }

    public class DanhSachYeuCauTiepNhanIds
    {
        public List<long> Id { get; set; }      
    }

}
