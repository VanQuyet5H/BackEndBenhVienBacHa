using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Helpers;
namespace Camino.Core.Domain.ValueObject.KhoDuocPhams
{
    //public class ThongTinLyDoHuyNhapKhoDuocPham
    //{
    //    public long YeuCauNhapKhoDuocPhamId { get; set; }
    //    public string LyDoHuy { get; set; }
    //}

    public class ThongTinDuyetHoanTraDuocPham
    {
        public string SoPhieu { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public long KhoCanHoanTraId { get; set; }
        public string TenKhoCanHoanTra { get; set; }
        public long KhoNhanHoanTraId { get; set; }
        public string TenKhoNhanHoanTra { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay { get; set; }
        public bool? TinhTrang { get; set; }
        public string TinhTrangDuyet => TinhTrang == null ? "Đang chờ duyệt" : TinhTrang == true ? "Đã duyệt" : "Từ chối duyệt";
        public string ClassTrangThai => TinhTrang == null ? "dang_cho_duyet" : TinhTrang == true ? "da_duyet" : "tu_choi_duyet";
        public long? NhanVienDuyetId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay { get; set; }
        public string GhiChu { get; set; }
        public long? NhanVienTraId { get; set; }
        public string TenNhanVienTra { get; set; }
        public long? NhanVienNhanId { get; set; }
        public string TenNhanVienNhan { get; set; }
        public string LyDoHuy { get; set; }
    }

    public class DanhSachDuyetHoanTraDuocPhamVo : GridItem
    {
        public string SoPhieu { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public string TenNhanVienYeuCau { get; set; }
        public long KhoCanHoanTraId { get; set; }
        public string TenKhoCanHoanTra { get; set; }
        public long KhoNhanHoanTraId { get; set; }
        public string TenKhoNhanHoanTra { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay { get; set; }
        public bool? TinhTrang { get; set; }
        public string TinhTrangDuyet => TinhTrang == null ? "Đang chờ duyệt" : TinhTrang == true ? "Đã duyệt" : "Từ chối duyệt";
        public string ClassTrangThai => TinhTrang == null ? "dang_cho_duyet" : TinhTrang == true ? "da_duyet" : "tu_choi_duyet";
        public long? NhanVienDuyetId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay { get; set; }
    }

    public class DanhSachDuyetHoanTraDuocPhamChiTietVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }

        public string Nhom { get; set; }
        public string Ma { get; set; }
        public string DuocPham { get; set; }
        public string DonViTinh { get; set; }
        public bool LoaiBHYT { get; set; }
        public string LoaiBHYTDisplay => LoaiBHYT ? "BHYT" : "Không BHYT";
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung.ApplyFormatDateTime();
        public double SoLuongHoanTra { get; set; }
        public string SoLuongHoanTraDisplay => SoLuongHoanTra.ApplyNumber();
        //public string HopDongThau { get; set; }
        //public string Nhom { get; set; }
        //public string HanSuDung { get; set; }
        //public string MaVach { get; set; }
        //public string SLConLaiHD { get; set; }
        //public string SoLuongTra { get; set; }
        //public string VAT { get; set; }
        //public decimal DonGiaNhap { get; set; }
        ////public decimal GiaBan { get; set; }
        //public string ThapGia { get; set; }
        ////public long? KhoViTriId { get; set; }
        //public string KhoViTri { get; set; }
        ////public int? TiLeBHYTThanhToan { get; set; }
        //public double SoLuongTrongHopDong { get; set; }
        //public double SoLuongTrongHopDongDaCap { get; set; }
    }

    public class HoanTraDuocPhamSearch
    {
        public bool DangChoDuyet { get; set; }
        public bool TuChoiDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeYeuCau { get; set; }
        public RangeDate RangeDuyet { get; set; }
    }

    public class DuyetHoanTraDuocPhamExportExcel : GridItem
    {
        public DuyetHoanTraDuocPhamExportExcel()
        {
            DuyetHoanTraDuocPhamExportExcelChild = new List<DuyetHoanTraDuocPhamExportExcelChild>();
        }

        [Width(30)]
        public string SoPhieu { get; set; }
        [Width(30)]
        public string TenNhanVienYeuCau { get; set; }
        [Width(30)]
        public string TenKhoCanHoanTra { get; set; }
        [Width(30)]
        public string TenKhoNhanHoanTra { get; set; }
        [Width(30)]
        public string NgayYeuCauDisplay { get; set; }
        [Width(30)]
        public bool? TinhTrang { get; set; }
        [Width(30)]
        public string TinhTrangDuyet => TinhTrang == null ? "Đang chờ duyệt" : TinhTrang == true ? "Đã duyệt" : "Từ chối duyệt";
        [Width(30)]
        public string ClassTrangThai => TinhTrang == null ? "dang_cho_duyet" : TinhTrang == true ? "da_duyet" : "tu_choi_duyet";
        [Width(30)]
        public string TenNhanVienDuyet { get; set; }
        [Width(30)]
        public DateTime? NgayDuyet { get; set; }
        [Width(30)]
        public string NgayDuyetDisplay { get; set; }


        public List<DuyetHoanTraDuocPhamExportExcelChild> DuyetHoanTraDuocPhamExportExcelChild { get; set; }
    }

    public class DuyetHoanTraDuocPhamExportExcelChild
    {
        [TitleGridChild("Mã")]
        public string Ma { get; set; }
        [TitleGridChild("Tên Dược Phẩm")]
        public string DuocPham { get; set; }
        [TitleGridChild("ĐVT")]
        public string DonViTinh { get; set; }
        [TitleGridChild("Loại BHYT")]
        public string LoaiBHYTDisplay { get; set; }
        [TitleGridChild("Số Lô")]
        public string SoLo { get; set; }
        [TitleGridChild("Hạn Sử Dụng")]
        public string HanSuDungDisplay { get; set; }
        [TitleGridChild("Số Lượng Hoàn Trả")]
        public double SoLuongHoanTra { get; set; }
    }
    public class PhieuHoanTraDuocPhamData
    {
        public string LogoUrl { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string KhoTraLai { get; set; }
        public string KhoNhan { get; set; }
        public string ThuocVatTu { get; set; } // content
        public string NgayLapPhieu { get; set; }
        public string SoPhieu { get; set; }
        //public string Ngay { get; set; }
        //public string Thang { get; set; }
        //public string Nam { get; set; }
        public string NgayThangNam { get; set; }


    }
    public class PhieuHoanTraDuocPhamChiTietData
    {
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string NuocSX { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string TenThuoc => Ten + " " + (!string.IsNullOrEmpty(HamLuong) ? HamLuong : " ") + " "
                        + (!string.IsNullOrEmpty(NuocSX) ? "(" + NuocSX + ")" : " ") + (!string.IsNullOrEmpty(SoLo) ? SoLo : " ") + " - " + HanSuDung.ApplyFormatDate();
        public string DVT { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)SoLuong * DonGia;
        public string GhiChu { get; set; }
        public string KhoTraLai { get; set; }
        public string KhoNhan { get; set; }
        public string SoPhieu { get; set; }
        public DateTime? CreatedOn { get; set; }
    }

    public class PhieuHoanTraDuocPhamVatTu
    {
        public long YeuCauHoanTraDuocPhamVatTuId { get; set; }
        public bool LaDuocPham { get; set; }
        public bool LaTuTruc { get; set; }
        public bool DuocDuyet { get; set; }
    }

    public class PhieuHoanTraDuocPhamVatTuData
    {
        public long YeuCauHoanTraDuocPhamVatTuId { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string SoPhieu { get; set; }
        public string ThuocVatTu { get; set; } // content
        public string KhoaPhong { get; set; }
        public string NgayThangNam { get; set; }
        public string KhoTraLai { get; set; }
        public string KhoNhan { get; set; }
        public string DienGiai { get; set; }
        public string GhiChu { get; set; }
        public string NgayLapPhieu { get; set; }
        public int CongKhoan { get; set; }
        public string TongCong { get; set; }
    }
    public class PhieuHoanTraDuocPhamVatTuTuTrucChiTietData
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string SoLoHSD => !string.IsNullOrEmpty(SoLo) ? SoLo + " - " + HanSuDung.ApplyFormatDate() : HanSuDung.ApplyFormatDate();
        public string KhoTraLai { get; set; }
        public string KhoNhan { get; set; }
        //public string TenThuoc => Ten + " " + (!string.IsNullOrEmpty(HamLuong) ? HamLuong : " ") + " "
        //                + (!string.IsNullOrEmpty(NuocSX) ? "(" + NuocSX + ")" : " ") + (!string.IsNullOrEmpty(SoLo) ? SoLo : " ") + " - " + HanSuDung.ApplyFormatDate();
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)SoLuong * DonGia;
        public string GhiChu { get; set; }
        public string SoPhieu { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? DuocDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public DateTime NgayYeuCau { get; set; }

    }

    public class PhieuHoanTraDuocPhamVatTuBenhNhanChiTietData
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public string SoLoHSD => !string.IsNullOrEmpty(SoLo) ? SoLo + " - " + HanSuDung?.ApplyFormatDate() : HanSuDung?.ApplyFormatDate();
        public DateTime? HanSuDung { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; } //=> CalculateHelper.TinhDonGiaBan(DonGiaNhap, TiLeTheoThapGia, VAT);
        public bool? KhongTinhPhi { get; set; }
        public decimal ThanhTien => KhongTinhPhi != true ? (decimal)SoLuong * DonGia : 0;
        public string GhiChu { get; set; }
        public string SoPhieu { get; set; }
        public string KhoaPhong { get; set; }
        public string KhoNhan { get; set; }
        public string HoTenBenhNhan { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? DuocDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public DateTime NgayYeuCau { get; set; }
    }

}
