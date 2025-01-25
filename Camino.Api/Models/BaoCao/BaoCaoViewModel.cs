using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BaoCao
{
    public class BaoCaoViewModel 
    {
        public int STT { get; set; }
        public string HoVaTenBenhNhan { get; set; }
        public string KhoaPhong { get; set; }
        public string DichVu { get; set; }

        public string Thang { get; set; }
        public decimal? DoanhThuThang { get; set; }
        public decimal? MienGiamThang { get; set; }
        public decimal? KhacThang { get; set; }
        public decimal? BHYTThang { get; set; }
        public decimal? DoanhThuThuanThang { get; set; }
        public decimal? DoanhThuKy { get; set; }
        public string KySoSanh { get; set; }
        public decimal? MienGiamKy { get; set; }
        public decimal? KhacKy { get; set; }
        public decimal? BHYTKy { get; set; }
        public decimal? DoanhThuThuanKy { get; set; }
    }
  
    public class SearchDate
    {
      public string DateSearchString { get; set; }
    }
    public class RangeDate
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
    public class Search
    {
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
    }
    public class BaoCaoTHDTBacSi
    {
        public int STT { get; set; }
        public string HoVaTenBS { get; set; }
        public decimal? DoanhThu { get; set; }
        public decimal MienGiam { get; set; }
        public decimal Khac { get; set; }
        public decimal? BHYT { get; set; }
        public decimal? ThucThu { get; set; }

    }
    public class BaoCaoDoanhThu
    {
        public int STT { get; set; }
        public string MaBN { get; set; }
        public string HovaTen { get; set; }
        public int NamSinh { get; set; }
        public bool? GioiTinh { get; set; }
        public int SoBenhAn { get; set; }
        public string NoiDung { get; set; }
        public string Ngay { get; set; }
        public string NguoiGioiThieu { get; set; }
        public decimal KhamBenh { get; set; }
        public decimal XetNghiem { get; set; }
        public decimal NoiSoi { get; set; }
        public decimal NoiSoiTMH { get; set; }
        public decimal SieuAm { get; set; }
        public decimal XQuang { get; set; }
        public decimal CTScan { get; set; }
        public decimal MRI { get; set; }
        public decimal DienTimDienNao { get; set; }
        public decimal TDCNDoLoangXuong { get; set; }
        public decimal ThuThuat { get; set; }
        public decimal PhauThuat { get; set; }
        public decimal NgayGiuong { get; set; }
        public decimal DVKhac { get; set; }
        public decimal Thuoc { get; set; }
        public decimal VTYT { get; set; }
        public decimal TongCong { get; set; }
    }
    public class TheoDoiTinhHinhThanhToanVienPhi
    {
        public string SoBA { get; set; }
        public string HovaTen { get; set; }
        public string NgayVaoVien { get; set; }
        public string MaYT { get; set; }
        public int SoBHYT { get; set; }
        public string TenDT { get; set; }
        public decimal TongVienPhi { get; set; }
        public decimal TamUng { get; set; }
        public decimal ChenhLech { get; set; }
        public string GhiChu { get; set; }
    }
}
