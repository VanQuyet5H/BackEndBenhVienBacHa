using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuSoKet15NgayDieuTri
{
    public class PhieuSoKet15NgayDieuTriVo : GridItem
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string  DanhGiaKetQua  {get;set;}
        public string HuongDieuTriTiep { get;set;}
        public string TaiKhoanDangNhap  {get;set;}
        public DateTime? NgayThucHien {get;set;}
        public string BSDieuTri  {get;set;}
        public string TruongKhoa {get;set;}
    
        public string SoYTe  {get;set;}
        public string BV {get;set;}
        public string SoVaoVien   {get;set;}
        public string HoTenNgBenh   {get;set;}
        public string TuoiNgBenh   {get;set;}
        public string GTNgBenh   {get;set;}
        public string DiaChi   {get;set;}
        public string Khoa   {get;set;}
        public string Buong   {get;set;}
        public string Giuong   {get;set;}
        public string ChanDoan   {get;set;}
        public string DienBienLS   {get;set;}
        public string XetNghiemCLS   {get;set;}
        public string QuaTrinhDieuTri   {get;set;}
        public string DanhGiaKQ   {get;set;}
        public string HoTenTruongKhoa   {get;set;}
        public string HoTenBacSi   {get;set;}
        public string NgayThucHienText { get; set; }
        public string NgayThucHienString { get; set; }

        public string TuNgayString { get; set; }
        public string DenNgayString { get; set; }
        public bool? NhanVienTrongBVHayNgoaiBV { get; set; }
        public bool? NhanVienTrongBVHayNgoaiBVTruongKhoa { get; set; }

        public long? BSDieuTriId { get; set; }

        public long? TruongKhoaId { get; set; }

        public string HocHamHocViBsDieuTri { get; set; }

        public string HocHamHocViTruongKhoa { get; set; }
    }
    public class DanhSachSoKet15NgayGridVo : GridItem
    {
        public DateTime TuNgay { get; set; }
        public string TuNgayString => TuNgay.ApplyFormatDate();
        public DateTime DenNgay { get; set; }
        public string DenNgayString => DenNgay.ApplyFormatDate();
        public string ThongTinHoSo { get; set; }
    }
    public class PhieuInSoKet15NgayDieuTriVo : GridItem
    {
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string DanhGiaKetQua { get; set; }
        public string HuongDieuTriVaTienLuong { get; set; }
        public string TaiKhoanDangNhap { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string BSDieuTri { get; set; }
        public string TruongKhoa { get; set; }

        public string SoYTe { get; set; }
        public string BV { get; set; }
        public string SoVaoVien { get; set; }
        public string HoTenNgBenh { get; set; }
        public string TuoiNgBenh { get; set; }
        public string GTNgBenh { get; set; }
        public string DiaChi { get; set; }
        public string Khoa { get; set; }
        public string Buong { get; set; }
        public string Giuong { get; set; }
        public string ChanDoan { get; set; }
        public string DienBienLS { get; set; }
        public string XetNghiemCLS { get; set; }
        public string QuaTrinhDieuTri { get; set; }
        public string DanhGiaKQ { get; set; }
        public string HoTenTruongKhoa { get; set; }
        public string HoTenBacSi { get; set; }
    }
    public class PhieuDieuTriVaServicesHttpParams15Ngay
    {
        public long YeuCauTiepNhanId { get; set; }
        public long NoiTruHoSoKhacId { get; set; }

        public string HostingName { get; set; }

        public bool? Header { get; set; }
    }
    public class In15NgayVo : GridItem
    {
        public string KhoaDangIn { get; set; }
        public string MaTN { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string HoTenNgBenh { get; set; }
        public string TuoiNgBenh { get; set; }
        public string GT { get; set; }

        public string DiaChi { get; set; }
        public string Khoa { get; set; }
        public string Buong { get; set; }
        public string Giuong { get; set; }
        public string ChanDoan { get; set; }
        public string DienBienLS { get; set; }
        public string XetNghiemCLS { get; set; }
        public string QuaTrinhDieuTri { get; set; }
        public string DanhGiaKetQua { get; set; }
        public string HuongDieuTriTiep { get; set; }
        public string NgayK { get; set; }
        public string ThangK { get; set; }
        public string NamK { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string HoTenTruongKhoa { get; set; }
        public string HoTenBacSi { get; set; }
    }
    public class DataInPhieuDieuTri15NgayVaSerivcesVo
    {
        public string MaSoTiepNhan { get; set; }

        public string Khoa { get; set; }

        public string MaBn { get; set; }

        public string NhomMau { get; set; }

        public string HoTenNgBenh { get; set; }

        public int? NamSinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NgaySinh { get; set; }

        public int TuoiNgBenh => NamSinh != null ? DateTime.Now.Year - NamSinh.GetValueOrDefault() : 0;

        public string Cmnd { get; set; }

        public string GTNgBenh { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string Buong { get; set; }

        public string Giuong { get; set; }

        public string ChanDoan { get; set; }

        public string DuKienPPDieuTri { get; set; }

        public string TienLuong { get; set; }

        public string NhungDieuCanLuuY { get; set; }

        public string BNSuDungBHYT { get; set; }

        public string ThongTinVeGiaDV { get; set; }

        public int Ngay { get; set; }

        public int Thang { get; set; }

        public int Nam { get; set; }

        public string HoTenBacSi { get; set; }

        public string ChanDoanRaVien { get; set; }

        public string ChanDoanVaoVien { get; set; }

        public DateTime? NgayVaoVien { get; set; }

        public DateTime? NgayRaVien { get; set; }
        public Enums.LoaiBenhAn LoaiBenhAn { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
    }
}
