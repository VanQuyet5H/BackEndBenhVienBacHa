using Camino.Core.Domain.ValueObject.Grid;
using System;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.YeuCauTraThuocTuBenhNhan
{
    public class YeuCauTraThuocTuBenhNhanGridVo : GridItem
    {
        public YeuCauTraThuocTuBenhNhanGridVo()
        {
            TraDuocPhamChiTietVos = new List<TraDuocPhamChiTietGridVo>();
        }
        public long? KhoTraId { get; set; }
        public string TenKho { get; set; }
        public long? KhoaHoanTraId { get; set; }
        public string TenKhoa { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        public string NhanVienYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay => NgayYeuCau?.ApplyFormatDateTimeSACH();
        public string GhiChu { get; set; }
        public bool? DuocDuyet { get; set; }
        public int? TinhTrang => DuocDuyet == false ? 2 : (DuocDuyet == true ? 1 : 0); // 0: chờ duyệt, 1: được duyệt, 2: từ chối
        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay => NgayDuyet?.ApplyFormatDateTimeSACH();
        public long? NhanVienDuyetId { get; set; }
        public string NhanVienDuyet { get; set; }
        //public string LyDoKhongDuyet { get; set; }
        public string SoPhieu { get; set; }
        public bool? ChoDuyet { get; set; }
        public bool? DaDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public List<TraDuocPhamChiTietGridVo> TraDuocPhamChiTietVos { get; set; }
    }



    public class TraDuocPhamChiTietGridVo : GridItem
    {
        public TraDuocPhamChiTietGridVo()
        {
            TraDuocPhamBenhNhanChiTietVos = new List<TraDuocPhamBenhNhanChiTietGridVo>();
        }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public string YeuCauDuocPhamBenhVienIds { get; set; }
        public long? YeuCauTraDuocPhamTuBenhNhanId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public string Nhom => LaDuocPhamBHYT == true ? "BHYT" : "Không BHYT";
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public double? SLChiDinh { get; set; }
        public double? SLDaTra { get; set; }
        public double? SLDaTraLanNay { get; set; }
        public double? TongSoLuongChiDinh { get; set; }
        public double? TongSoLuongDaTra { get; set; }
        public double? TongSoLuongTraLanNay { get; set; }
        public string TongSLChiDinh => TongSoLuongChiDinh.ApplyNumber();
        public string TongSLDaTra => TongSoLuongDaTra.ApplyNumber();
        public string TongSLDaTraLanNay => TongSoLuongTraLanNay.ApplyNumber();
        public long? KhoaHoanTraId { get; set; }
        public long? KhoTraId { get; set; }
        public bool IsCreate { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public List<TraDuocPhamBenhNhanChiTietGridVo> TraDuocPhamBenhNhanChiTietVos { get; set; }
        public bool? DuocDuyet { get; set; }
    }

    public class TraDuocPhamBenhNhanChiTietGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? NgayDieuTri { get; set; }
        public string NgayDieuTriDisplay => NgayDieuTri?.ApplyFormatDate();
        public DateTime? NgayTra { get; set; }
        public string NgayTraDisplay => NgayTra?.ApplyFormatDate();
        public string BenhNhan { get; set; }
        public string NhanVienYeuCau { get; set; }
        public double? SoLuongChiDinh { get; set; }
        public string SLChiDinh => SoLuongChiDinh.ApplyNumber();
        public double? SoLuongDaTra { get; set; }
        public string SLDaTra => SoLuongDaTra.ApplyNumber();
        public double? SoLuongTraLanNay { get; set; }
        public string SLTraLanNay => SoLuongTraLanNay.ApplyNumber();
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal ThanhTien => KhongTinhPhi != true ? DonGia * (decimal)SoLuongTraLanNay.GetValueOrDefault() : 0;
        public bool? DuocDuyet { get; set; }
    }

    public class ThongTinKhoaHoanTra
    {
        public long KhoaHoanTraId { get; set; }
        public string TenKhoaTra { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
    }


    ///////////Vật tư
    ///
    public class YeuCauTraVatTuTuBenhNhanGridVo : GridItem
    {
        public YeuCauTraVatTuTuBenhNhanGridVo()
        {
            TraVatTuChiTietVos = new List<TraVatTuChiTietGridVo>();
        }
        public long? KhoTraId { get; set; }
        public string TenKho { get; set; }
        public long? KhoaHoanTraId { get; set; }
        public string TenKhoa { get; set; }
        public long? NhanVienYeuCauId { get; set; }
        public string NhanVienYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay => NgayYeuCau?.ApplyFormatDateTimeSACH();
        public string GhiChu { get; set; }
        public bool? DuocDuyet { get; set; }
        public int? TinhTrang => DuocDuyet == false ? 2 : (DuocDuyet == true ? 1 : 0); // 0: chờ duyệt, 1: được duyệt, 2: từ chối
        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay => NgayDuyet?.ApplyFormatDateTimeSACH();
        public long? NhanVienDuyetId { get; set; }
        public string NhanVienDuyet { get; set; }
        //public string LyDoKhongDuyet { get; set; }
        public string SoPhieu { get; set; }
        public bool? ChoDuyet { get; set; }
        public bool? DaDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public List<TraVatTuChiTietGridVo> TraVatTuChiTietVos { get; set; }
    }

    public class TraVatTuChiTietGridVo : GridItem
    {
        public TraVatTuChiTietGridVo()
        {
            TraVatTuBenhNhanChiTietVos = new List<TraVatTuBenhNhanChiTietGridVo>();
        }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public string YeuCauVatTuBenhVienIds { get; set; }
        public long? YeuCauTraVatTuTuBenhNhanId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public string Nhom => LaVatTuBHYT == true ? "BHYT" : "Không BHYT";
        public string DVT { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public double? SLChiDinh { get; set; }
        public double? SLDaTra { get; set; }
        public double? SLDaTraLanNay { get; set; }
        public double? TongSoLuongChiDinh { get; set; }
        public double? TongSoLuongDaTra { get; set; }
        public double? TongSoLuongTraLanNay { get; set; }
        public string TongSLChiDinh => TongSoLuongChiDinh.ApplyNumber();
        public string TongSLDaTra => TongSoLuongDaTra.ApplyNumber();
        public string TongSLDaTraLanNay => TongSoLuongTraLanNay.ApplyNumber();
        public long? KhoaHoanTraId { get; set; }
        public long? KhoTraId { get; set; }
        public bool IsCreate { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public List<TraVatTuBenhNhanChiTietGridVo> TraVatTuBenhNhanChiTietVos { get; set; }
        public bool? DuocDuyet { get; set; }
    }

    public class TraVatTuBenhNhanChiTietGridVo : GridItem
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? NgayDieuTri { get; set; }
        public string NgayDieuTriDisplay => NgayDieuTri?.ApplyFormatDate();
        public DateTime? NgayTra { get; set; }
        public string NgayTraDisplay => NgayTra?.ApplyFormatDate();
        public string BenhNhan { get; set; }
        public string NhanVienYeuCau { get; set; }
        public double? SoLuongChiDinh { get; set; }
        public string SLChiDinh => SoLuongChiDinh.ApplyNumber();
        public double? SoLuongDaTra { get; set; }
        public string SLDaTra => SoLuongDaTra.ApplyNumber();
        public double? SoLuongTraLanNay { get; set; }
        public string SLTraLanNay => SoLuongTraLanNay.ApplyNumber();
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal ThanhTien => KhongTinhPhi != true ? DonGia * (decimal)SoLuongTraLanNay.GetValueOrDefault() : 0 ;
        public bool? DuocDuyet { get; set; }
    }

    public class PhieuTraThuoc
    {
        public long YeuCauTraDuocPhamTuBenhNhanId { get; set; }
        public string HostingName { get; set; }
    }

    public class PhieuTraVatTu
    {
        public long YeuCauTraVatTuTuBenhNhanId { get; set; }
        public string HostingName { get; set; }
    }

    public class PhieuHoanTraVatTuTuBenhNhanData
    {
        public string LogoUrl { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string KhoaTraLai { get; set; }
        public string KhoNhan { get; set; }
        public string ThuocVatTu { get; set; } // content
        public string NgayLapPhieu { get; set; }
        public string SoPhieu { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string NgayThangNam { get; set; }
        //public string Thang { get; set; }
        //public string Nam { get; set; }

    }
    public class PhieuHoanTraVatTuTuBenhNhanChiTietData
    {
        public string HoTen { get; set; }
        public string Ten { get; set; }
        public string NuocSX { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string TenVatTu => Ten + " " + (!string.IsNullOrEmpty(NuocSX) ? "(" + NuocSX + ")" : " ") + (!string.IsNullOrEmpty(SoLo) ? SoLo : " ") + " - " + HanSuDung?.ApplyFormatDate();
        public string DVT { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public decimal ThanhTien => KhongTinhPhi != true ? (decimal)SoLuong * DonGia : 0;
        public string GhiChu { get; set; }
        public string KhoaTraLai { get; set; }
        public string KhoNhan { get; set; }
        public string SoPhieu { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? KhongTinhPhi { get; set; }

    }

    public class PhieuHoanTraThuocTuBenhNhanChiTietData
    {
        public string HoTen { get; set; }
        public string Ten { get; set; }
        public string NuocSX { get; set; }
        public string SoLo { get; set; }
        public string HamLuong { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string TenThuoc => Ten + " " + (!string.IsNullOrEmpty(HamLuong) ? "(" + HamLuong + ")" : " ") + " " + (!string.IsNullOrEmpty(NuocSX) ? "(" + NuocSX + ")" : " ") + (!string.IsNullOrEmpty(SoLo) ? SoLo : " ") + " - " + HanSuDung?.ApplyFormatDate();
        public string DVT { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public bool? KhongTinhPhi { get; set; }
        public decimal ThanhTien => KhongTinhPhi != true ? (decimal)SoLuong * DonGia : 0;
        public string GhiChu { get; set; }
        public string KhoaTraLai { get; set; }
        public string KhoNhan { get; set; }
        public string SoPhieu { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string HoatChat { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public Enums.LoaiThuocTheoQuanLy? LoaiThuocTheoQuanLy { get; set; }
        public bool LaDuocPhamHayVatTu { get; set; }

    }


}
