using Camino.Core.Domain.ValueObject.Grid;
using System;
using Camino.Core.Helpers;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Core.Domain.ValueObject.YeuCauDieuChuyenDuocPhams
{
    public class YeuCauDieuChuyenDuocPhamVo : GridItem
    {
        public YeuCauDieuChuyenDuocPhamVo()
        {
            YeuCauDieuChuyenDuocPhamChiTietVos = new List<YeuCauDieuChuyenDuocPhamChiTietVo>();
        }
        public string TenKhoNhap { get; set; }
        public string TenKhoXuat { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay => NgayYeuCau?.ApplyFormatDateTimeSACH();
        public bool? DuocKeToanDuyet { get; set; }
        public int? TinhTrang => DuocKeToanDuyet == false ? 2 : (DuocKeToanDuyet == true ? 1 : 0); // 0: chờ duyệt, 1: được duyệt, 2: từ chối
        public string TinhTrangDisplay => TinhTrang == 0 ? "Chờ duyệt" : (TinhTrang == 1 ? "Đã duyệt" : "Từ chối");

        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay => NgayDuyet?.ApplyFormatDateTimeSACH();
        public string TenNhanVienDuyet { get; set; }
        public string SoPhieu { get; set; }
        public bool? ChoDuyet { get; set; }
        public bool? DaDuyet { get; set; }
        public bool? TuChoiDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDates RangeFromDate { get; set; }
        public List<YeuCauDieuChuyenDuocPhamChiTietVo> YeuCauDieuChuyenDuocPhamChiTietVos { get; set; }
    }


    public class YeuCauDieuChuyenDuocPhamChiTietVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public double SoLuongDieuChuyen { get; set; }
        public string SoLuongDieuChuyenDisplay => SoLuongDieuChuyen.ApplyNumber();
        public bool LaDuocPhamBHYT { get; set; }
        public string Loai => LaDuocPhamBHYT ? "BHYT" : "Không BHYT";
        public string Nhom { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung.ApplyFormatDate();
        //public PhuongPhapTinhGiaTriTonKho? PhuongPhapTinhGiaTriTonKho { get; set; }
    }

    public class YeuCauDieuChuyenThuocExportExcel
    {
        public YeuCauDieuChuyenThuocExportExcel()
        {
            YeuCauDieuChuyenThuocExportExcelChild = new List<YeuCauDieuChuyenThuocExportExcelChild>();
        }
        public long Id { get; set; }
        [Width(30)]
        public string SoPhieu { get; set; }
        [Width(30)]
        public string TenKhoNhap { get; set; }
        [Width(30)]
        public string TenKhoXuat { get; set; }
        [Width(30)]
        public string TenNhanVienYeuCau { get; set; }
        [Width(30)]
        public string NgayYeuCauDisplay { get; set; }
        [Width(30)]
        public string TenNhanVienDuyet { get; set; }
        [Width(30)]
        public string TinhTrangDisplay { get; set; }
        [Width(30)]
        public string NgayDuyetDisplay { get; set; }
        public List<YeuCauDieuChuyenThuocExportExcelChild> YeuCauDieuChuyenThuocExportExcelChild { get; set; }
    }

    public class YeuCauDieuChuyenThuocExportExcelChild
    {
        [Group]
        public string Nhom { get; set; }
        [TitleGridChild("Dược phẩm")]
        public string Ten { get; set; }
        [TitleGridChild("Hoạt chất")]
        public string HoatChat { get; set; }
        [TitleGridChild("ĐVT")]
        public string DVT { get; set; }
        [TitleGridChild("Số lô")]
        public string SoLo { get; set; }
        [TitleGridChild("Hạn sử dụng")]
        public string HanSuDungDisplay { get; set; }
        [TitleGridChild("Số lượng điều chuyển")]
        public string SoLuongDieuChuyenDisplay { get; set; }
    }

    public class YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVos
    {
        public YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVos()
        {
            YeuCauDieuChuyenDuocPhamChiTiets = new List<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo>();
        }
        public List<YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo> YeuCauDieuChuyenDuocPhamChiTiets { get; set; }
    }

    public class YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        //public string Loai => LaDuocPhamBHYT ? "BHYT" : "Không BHYT";
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon.ApplyNumber();
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string TenNhom { get; set; }
        public string Ma { get; set; }
        public string SoDangKy { get; set; }
        public string HamLuong { get; set; }
        public string SoLo { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaNhap { get; set; }

        public double SoLuongDieuChuyen { get; set; }
        public decimal ThanhTien => (decimal)SoLuongDieuChuyen * DonGia;
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
        public long? XuatKhoDuocPhamChiTietViTriId { get; set; }
        public long? KhoXuatId { get; set; }

    }

    public class YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVoSearch
    {
        public YeuCauDieuChuyenDuocPhamChiTietTheoKhoXuatVoSearch()
        {
            DuocPhamBenhViens = new List<DuocPhamBenhVienChiTietVo>();
        }
        public long? KhoXuatId { get; set; }

        public long? YeuCauDieuChuyenDuocPhamId { get; set; }
        public string SearchString { get; set; }
        public bool? HienThiCaThuocHetHan { get; set; }
        public List<DuocPhamBenhVienChiTietVo> DuocPhamBenhViens { get; set; }
    }

    public class DuocPhamBenhVienChiTietVo
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string SoLo { get; set; }
        public decimal DonGia { get; set; }
    }

    public class XuatKhoDieuChuyenKhoNoiBoDuocPhamChiTietVo
    {
        public double? SoLuongDieuChuyen { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public long? KhoXuatId { get; set; }
        public long? XuatKhoDuocPhamChiTietViTriId { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
        public bool? WillDelete { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public decimal? DonGia { get; set; }
    }

    public class YeuCauDieuChuyenDuocPhamChiTietData
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string Ma { get; set; }
        public string HamLuong { get; set; }
        public string SoLo { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaNhap { get; set; }
        public double SoLuongDieuChuyen { get; set; }
        public decimal ThanhTien => DonGia * Convert.ToDecimal(SoLuongDieuChuyen);
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
        public DateTime? CreateOn { get; set; }
        public string NgayTaoPhieu => CreateOn?.ApplyFormatNgayThangNam();
        public string NguoiNhan { get; set; }
        public string KhoNhap { get; set; }
        public string KhoXuat { get; set; }
        public string LyDoXuat { get; set; }
        public string DCKhoXuat { get; set; }
        public string NguoiLap { get; set; }
    }
    public class YeuCauDieuChuyenDuocPhamDataVo
    {
        public long YeuCauDieuChuyenDuocPhamId { get; set; }
        public string HostingName { get; set; }
    }
    public class YeuCauDieuChuyenDuocPhamData
    {
        public int CongKhoan { get; set; }
        public decimal TongCongDecimal { get; set; }
        public string TongCong => TongCongDecimal.ApplyFormatMoneyVND().Replace(" ₫", "");
        public string TongCongChu => ConvertNumberToStringCurrencyHelper.ApplytNumberToCurrencyString(TongCongDecimal);
        public string LogoUrl { get; set; }
        public string DuocPhamHoacVatTus { get; set; }
        public string NgayTaoPhieu { get; set; }
        public string NguoiNhan { get; set; }
        public string KhoNhap { get; set; }
        public string KhoXuat { get; set; }
        public string LyDoXuat { get; set; }
        public string DCKhoXuat { get; set; }
        public string NguoiLap { get; set; }
        public string NgayThangNam { get; set; }
    }
}
