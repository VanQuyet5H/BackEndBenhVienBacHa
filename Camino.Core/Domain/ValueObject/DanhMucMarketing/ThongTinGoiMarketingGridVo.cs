using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DanhMucMarketing
{
    public class DanhSachMarketingSearchSearch
    {
        public bool DangChoNhanTien { get; set; }
        public bool DangChoThanhToan { get; set; }
        public bool DaThanhToan { get; set; }
        public string SearchString { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }

    public class DanhSachMarketingGridVo : GridItem
    {
        //MaBenhNhan, TenBenhNhan, NamSinh, GioiTinh, DienThoai, ChungMinhThu, DiaChi
        public int STT { get; set; }
        public string MaBenhNhan { get; set; }
        public string TenBenhNhan { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string DienThoai { get; set; }
        public string DienThoaiDisplay { get; set; }
        public string ChungMinhThu { get; set; }
        public string DiaChi { get; set; }
        public string NgayTaoDisplay { get; set; }
        public DateTime? NgayTao { get; set; }
        public bool EnableDeleteButton { get; set; }
    }


    public class DanhSachMarketingChildGridVo : GridItem
    {
        //ChuongTrinhGoiMarketing, TongTienTT, TrangThaiTT, TrangThaiSuDung, TrangThaiNhanQua
        public int STT { get; set; }
        public string ChuongTrinhGoiMarketing { get; set; }
        public string TongTienTT { get; set; }
        public string TongTienTTDisplay { get; set; }
        public string TrangThaiTT { get; set; }
        public string TrangThaiSuDung { get; set; }
        public string TrangThaiNhanQua { get; set; }

        public int TongSoQua { get; set; }
        public int TongSoQuaDaXuat { get; set; }

        public long benhNhanId { get; set; }
        public long chuongTrinhGoiDichVuId { get; set; }

        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }

        public Enums.EnumTrangThaiYeuCauGoiDichVu TrangThai { get; set; }

        public DateTime? NgayDangKy { get; set; }
        public string NgayDangKyDisplay { get { return (NgayDangKy ?? DateTime.Now).ApplyFormatDateTime(); } }
        public bool? BoPhanMarketingDaNhanTien { get; set; }
    }

    public class ThongTinGoiMarketingGridVo : GridItem
    {
        public int STT { get; set; }
        public string TenGoi { get; set; }
        public string TenDisplay { get; set; }
        public bool IsChecked { get; set; }

        

        public decimal GiaTruocChietKhau { get; set; }
        public decimal GiaSauChietKhau { get; set; }
        public string GiaTruocChietKhauDisplay { get; set; }
        public string GiaSauChietKhauDisplay { get; set; }



        public double TiLeChietKhau => GiaTruocChietKhau != 0 ? (double)((GiaTruocChietKhau - GiaSauChietKhau) / GiaTruocChietKhau * 100) : 0;
        public string TiLeChietKhauDisplay { get; set; }

        public Enums.TrangThaiThanhToan? TrangThai { get; set; }
        public string TrangThaiDisplay { get; set; }

        public bool IsHaveGift { get; set; }
        public bool CoCacDichVuKhac { get; set; }

        public decimal TongCong { get; set; }
        public decimal GiaGoi { get; set; }
        public decimal ChuaThu => GiaGoi - BenhNhanDaThanhToan;
        public string ChuaThuDisplay => ChuaThu.ApplyFormatMoneyVND();
        public decimal BenhNhanDaThanhToan { get; set; }
        public string BenhNhanDaThanhToanDisplay => BenhNhanDaThanhToan.ApplyFormatMoneyVND();
        public decimal DangDung { get; set; }
        public decimal ConLai => BenhNhanDaThanhToan - DangDung;
        public string ConLaiDisplay => ConLai.ApplyFormatMoneyVND();

        public Enums.EnumTrangThaiYeuCauGoiDichVu? TrangThaiGoi { get; set; }
        public string TrangThaiGoiDisplay => TrangThaiGoi.GetDescription();

        public bool? GoiSoSinh { get; set; }
        public DateTime NgayDangKy { get; set; }
        public string NgayDangKyDisplay => NgayDangKy.ApplyFormatDateTimeSACH(); 
        public string NguoiDangKy { get; set; }
      
    }

    //public class QuaTangGoiMarketingGridVo : GridItem
    //{
    //    public int STT { get; set; }
    //    public string QuaTang { get; set; }
    //    public string SoLuongDisplay { get; set; }
    //    public int SoLuong { get; set; }
    //    public string GhiChu { get; set; }
    //}

    public class DichVuGoiMarketingGridVo : GridItem
    {
        public long DichVuId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public int STT { get; set; }
        public string TenNhomDichVu { get; set; }
        public Enums.EnumNhomGoiDichVu? NhomDichVu { get; set; }
        public long? NhomGiaDichVuBenhVienId { get; set; }

        public string TenDichVu { get; set; }
        public string Ma { get; set; }
        public string LoaiGiaDisplay { get; set; }
        public string SoLuongDisplay { get; set; }
        public int SoLuong { get; set; }
        public string DonGiaDisplay { get; set; }
        public string ThanhTienDisplay { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }


        public double TiLeChietKhau => GiaTruocChietKhau != 0 ? (double)((GiaTruocChietKhau - GiaSauChietKhau) / GiaTruocChietKhau * 100) : 0;
        public string TiLeChietKhauDisplay { get; set; }

        public decimal GiaTruocChietKhau { get; set; }
        public decimal GiaSauChietKhau { get; set; }
        public string GiaTruocChietKhauDisplay { get; set; }
        public string GiaSauChietKhauDisplay { get; set; }

        public double SoLuongDaDung { get; set; }
        public string SoLuongDaDungDisplay { get; set; }
        public Enums.TrangThaiThanhToan? TrangThaiThanhToan { get; set; }
        public string TrangThaiThanhToanDisplay { get { return TrangThaiThanhToan.GetDescription(); } }

        public bool IsChecked { get; set; }
        public bool IsActive { get; set; }
        //tai sao ? public int SoLuongDungLanNay { get; set; } = 1
        public int SoLuongDungLanNay { get; set; } = 1;

        public int SoLuongDungLanNayClone { get { return SoLuongDungLanNay; } }
        //get { return SoLuong - SoLuongDaDung - SoLuongDungLanNay; }
        public double SoLuongConLai
        {
            get; set;
        }
        public string ThuocGoi { get; set; }
        public string TenGoi { get; set; }

        public bool IsNhomTiemChung { get; set; }
    }
    public class QuaTangGoiMarketingGridVo : GridItem
    {
        public int STT { get; set; }
        public string Ten { get; set; }
        public string SoLuongDisplay { get; set; }
        public int SoLuong { get; set; }
        public string SoLuongTonDisplay { get; set; }
        public int SoLuongTon { get; set; }
        public string GhiChu { get; set; }

        public string SoPhieuXuat { get; set; }

        public long QuaTangId { get; set; }
        public long YeuCauGoiDichVuId { get; set; }
    }

    public class CacDichVuKhuyenMaiTrongGoiMarketingGridVo : GridItem
    {
        public int NhomId { get; set; }
        public string Nhom => NhomId == 1 ? "Dịch vụ khám bệnh".ToUpper() : (NhomId == 2 ? "Dịch vụ kỹ thuật".ToUpper() : "Dịch vụ giường".ToUpper());
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string LoaiGia { get; set; }
        public int SoLan { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaKhuyenMai { get; set; }
        public decimal ThanhTien => DonGiaKhuyenMai * SoLan;
        public string GhiChu { get; set; }
        public int SoNgaySuDung { get; set; }
        public string HanSuDung { get; set; }
        public int SoLanDaDung { get; set; }

    }

    public class CacDichVuTrongGoiMarketingGridVo : GridItem
    {
        public int NhomId { get; set; }
        public string Nhom => NhomId == 1 ? "Dịch vụ khám bệnh".ToUpper() : (NhomId == 2 ? "Dịch vụ kỹ thuật".ToUpper() : "Dịch vụ giường".ToUpper());
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string LoaiGia { get; set; }
        public int SoLan { get; set; }
        public decimal DonGiaTruocCK { get; set; }
        public decimal DonGiaSauCK { get; set; }
        public decimal ThanhTienTruocCK => DonGiaTruocCK * SoLan;
        public decimal ThanhTienSauCK => DonGiaSauCK * SoLan;
    }

    public class QuaTangDaXuat
    {
        public long QuaTangId { get; set; }
        public int SoLuongDaXuat { get; set; }
        public long? XuatKhoId { get; set; }
        public long? YeuCauGoiId { get; set; }
    }

    public class DanhSachNhapKhoCanCapNhatSoLuong
    {
        public long NhapKhoChiTietId { get; set; }
        public int SoLuongXuat { get; set; }
    }

    public class ThongTinInXuatQuaVo
    {
        public string TenNguoiNhanHang { get; set; }
        public string BoPhan { get; set; }
        public string LyDoXuatKho { get; set; }
        public string SoPhieuXuat { get; set; }
        public string XuatTaiKho { get; set; }
        public string DiaDiem { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string DanhSachThuoc { get; set; }
        public string Header { get; set; }

        public string SoLuongTong { get; set; }


    }

    public class ThongTinInXuatQuaChiTietVo
    {
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string SLDisplay { get; set; }
        public double SL { get; set; }
        //public string TongSLDisplay { get; set; }
        //public double? TongSL { get; set; }
    }


    public class ChonGoiMarketing
    {
        public long? GoiMarketingId { get; set; }
        public int? SoLuong { get; set; }
    }

  
    public class ThongTinChiPhiGoiDichVuVo
    {
        public long YeuCauGoiId { get; set; }
        public decimal? ChiPhi { get; set; }
    }
}
