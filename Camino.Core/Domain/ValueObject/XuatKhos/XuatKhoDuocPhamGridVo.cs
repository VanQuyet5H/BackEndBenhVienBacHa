using System;
using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.XuatKhos
{
    public class DuocPhamXuatGridVo : GridItem
    {
        //public string Id { get; set; }
        public string Id => $"{DuocPhamBenhVienId},{(LaDuocPhamBHYT ? "1" : "0")},{HanSuDung?.ToString("yyyyMMdd")},{SoLo},{DonGia}";
        public long DuocPhamBenhVienId { get; set; }
        public int STT { get; set; }
        public string TenDuocPham { get; set; }
        public string DVT { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string Loai { get { return LaDuocPhamBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay
        {
            get { return SoLuongTon.ApplyNumber(); }
        }
        public double SoLuongXuat { get; set; }

        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string TenNhom { get; set; }
        public string MaDuocPham { get; set; }
        public string SoDangKy { get; set; }

        //update 26/05/2021
        public string HamLuong { get; set; }
        public string SoLo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)SoLuongXuat * DonGia;
        public string SoLuong { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
    }

    public class XuatKhoDuocPhamGridVo : GridItem
    {
        public string KhoDuocPhamXuat { get; set; }
        public string KhoDuocPhamNhap { get; set; }

        public string SoPhieu { get; set; }
        public string LyDoXuatKho { get; set; }
        public string NguoiNhan { get; set; }
        public string NguoiXuat { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string NgayXuatDisplay => NgayXuat?.ApplyFormatDateTime();
    }

    public class XuatKhoDuocPhamKhacGridVo : GridItem
    {
        public string KhoDuocPhamXuat { get; set; }
        public string SoPhieu { get; set; }
        public string LyDoXuatKho { get; set; }
        public string TenNguoiNhan { get; set; }
        public string TenNguoiXuat { get; set; }
        public DateTime? NgayXuat { get; set; }
        public bool? CoNCC { get; set; }
        public string NgayXuatDisplay => NgayXuat?.ApplyFormatDateTime();
        public bool? ChoDuyet { get; set; }
        public bool? DaDuyet { get; set; }
        public bool? DuocDuyet { get; set; }
        public int? TinhTrang => DuocDuyet == null ? 0 : 1;
        public string TinhTrangDisplay => TinhTrang == 0 ? "Chờ duyệt" : "Đã duyệt xuất";
        public string SearchString { get; set; }
        public RangeDates RangeFromDate { get; set; }
    }

    public class XuatKhoDuocPhamChildrenGridVo : GridItem
    {
        public string Nhom { get; set; }
        public string DuocPham { get; set; }
        public string DVT { get; set; }
        public string Loai { get; set; }
        public string SoLuongTon { get; set; }
        public string SoLuongXuat { get; set; }
        public string SoPhieu { get; set; }
        public long? XuatKhoDuocPhamId { get; set; }
        public string SearchString { get; set; }

    }

    public class ThongTinXuatKhoDuocPhamVo
    {
        public ThongTinXuatKhoDuocPhamVo()
        {
            ThongTinXuatKhoDuocPhamChiTietVos = new List<ThongTinXuatKhoDuocPhamChiTietVo>();
        }
        public List<ThongTinXuatKhoDuocPhamChiTietVo> ThongTinXuatKhoDuocPhamChiTietVos { get; set; }
        public long KhoXuatId { get; set; }
        public long KhoNhapId { get; set; }
        public DateTime NgayXuat { get; set; }
        public long NguoiXuatId { get; set; }
        public long? NguoiNhanId { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiNhan { get; set; }
        public string TenNguoiNhan { get; set; }
        public string LyDoXuatKho { get; set; }
    }
    public class ThongTinXuatKhoDuocPhamChiTietVo
    {
        public string Id { get; set; }
        public double SoLuongXuat { get; set; }
        public string SoLo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)SoLuongXuat * DonGia;
        public DateTime HanSuDung { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDpBHYT { get; set; }
    }

    public class ThemDuocPham
    {
        public long? DuocPhamBenhVienId { get; set; }
        public int? ChatLuong { get; set; }
        public double? SoLuongXuat { get; set; }
        public double? SoLuongTon { get; set; }
        public long? KhoId { get; set; }

        public decimal? DonGia { get; set; }
        public int? VAT { get; set; }
        public int? ChietKhau { get; set; }

        public bool LaDuocPhamBHYT { get; set; }

        public long? NhomDuocPhamId { get; set; }

        public Enums.EnumLoaiKhoDuocPham? loaiKhoDuocPhamXuat { get; set; }
        public Enums.XuatKhoDuocPham? loaiXuatKho { get; set; }
    }
    public class ThemDuocPhamNoValidate
    {
        public long? DuocPhamId { get; set; }
        public int? ChatLuong { get; set; }
        public double? SoLuongXuat { get; set; }
        public double? SoLuongTon { get; set; }
        public long? KhoId { get; set; }
    }
    public class KhoTemplateVo
    {
        public string DisplayName { get; set; }
        public string HoatChat { get; set; }
        public string Ten { get; set; }
        public long KeyId { get; set; }
    }

    public class XuatKhoDuocPhamSearch
    {
        public string SearchString { get; set; }
        //public RangeDate RangeXuat { get; set; }
        public RangeDates RangeXuat { get; set; }

    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class ThongTinInXuatKhoDuocPhamVo
    {
        public string TenNguoiNhanHang { get; set; }
        public string BoPhan { get; set; }
        public string LyDoXuatKho { get; set; }
        public string XuatTaiKho { get; set; }
        public string DiaDiem { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string DanhSachThuoc { get; set; }
        public string Header { get; set; }
        public string LogoUrl { get; set; }

        public string SoLuongYeuCauTong { get; set; }

        public string SoLuongThucXuatTong { get; set; }

    }

    public class ThongTinInXuatKhoDuocPhamChiTietVo
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ma { get; set; }
        public string TenThuoc { get; set; }
        public string DVT { get; set; }
        public string SLYeuCau { get; set; }
        public double SLYC { get; set; }
        public string SLThucXuat { get; set; }
        public double SLTX { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string TenNhom { get; set; }
        public long? DuocPhamBenhVienPhanNhomChaId { get; set; }
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
    }

    public class NhaCCTheoKhoDuocPhamJsonVo
    {
        public long? Id { get; set; }
        public long? KhoId { get; set; }
    }

    public class SoCTTheoKhoDuocPhamJsonVo
    {
        public long? Id { get; set; }
        public long? NhaThauId { get; set; }
        public long? KhoId { get; set; }
    }
    public class YeuCauXuatKhoDuocPhamChiTietVoSearch
    {
        public YeuCauXuatKhoDuocPhamChiTietVoSearch()
        {
            DuocPhamBenhViens = new List<XuatKhacDuocPhamBenhVienChiTietVo>();
        }
        public long? KhoXuatId { get; set; }
        public string SearchString { get; set; }
        public long? NhaThauId { get; set; }
        public string SoChungTu { get; set; }
        public List<XuatKhacDuocPhamBenhVienChiTietVo> DuocPhamBenhViens { get; set; }
    }

    public class XuatKhacDuocPhamBenhVienChiTietVo
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
    }

    public class YeuCauXuatDuocPhamChiTietTheoKhoXuatVos
    {
        public YeuCauXuatDuocPhamChiTietTheoKhoXuatVos()
        {
            YeuCauXuatKhoDuocPhamChiTiets = new List<YeuCauXuatKhoDuocPhamGridVo>();
        }
        public List<YeuCauXuatKhoDuocPhamGridVo> YeuCauXuatKhoDuocPhamChiTiets { get; set; }
    }

    public class YeuCauXuatKhoDuocPhamGridVo : GridItem
    {
        public long DuocPhamBenhVienId { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon.ApplyNumber();
        public long? DuocPhamBenhVienPhanNhomId { get; set; }
        public string TenNhom { get; set; }
        public string Ma { get; set; }
        public string SoDangKy { get; set; }
        public string HamLuong { get; set; }
        public string SoLo { get; set; }
        public double SoLuongXuat { get; set; }
        public decimal DonGiaNhap { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
        public long? XuatKhoDuocPhamChiTietViTriId { get; set; }
        public long? KhoXuatId { get; set; }
        public long? NhaThauId { get; set; }
        public string SoChungTu { get; set; }
        public string SoPhieu { get; set; }
        public long? XuatKhoDuocPhamId { get; set; }
        public string SearchString { get; set; }

        public long? XetNghiemIdDauTienMayXetNghiem { get; set; }
        public string TenXetNghiemDauTienMayXetNghiem { get; set; }
    }
    public class CapNhatXuatKhoKhacResultVo
    {
        public long Id { get; set; }
        public byte[] LastModified { get; set; }
    }
    public class XuatKhoKhacDuocPhamChiTietVo
    {
        public double? SoLuongXuat { get; set; }
        public string Ma { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
        public long? KhoXuatId { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }

        public long? MayXetNghiemId { get; set; }
    }

    public class XuatKhoKhacDuocPhamRsVo : GridItem
    {
        public virtual byte[] LastModified { get; set; }
    }

    public class XuatKhoKhacLookupItem : LookupItemVo
    {
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
    }

    public class XuatKhoDuocPhamKhacExportExcel : GridItem
    {
        public XuatKhoDuocPhamKhacExportExcel()
        {
            XuatKhoDuocPhamKhacExportExcelChild = new List<XuatKhoDuocPhamKhacExportExcelChild>();
        }
        [Width(30)]
        public string SoPhieu { get; set; }
        [Width(30)]
        public string KhoDuocPhamXuat { get; set; }
        [Width(30)]
        public string TenNguoiXuat { get; set; }
        [Width(30)]
        public string NgayXuatDisplay { get; set; }
        [Width(30)]
        public string TenNguoiNhan { get; set; }
        [Width(30)]
        public string LyDoXuatKho { get; set; }
        [Width(30)]
        public string TinhTrangDisplay { get; set; }
        [Width(30)]
        public int? TinhTrang { get; set; }
        public List<XuatKhoDuocPhamKhacExportExcelChild> XuatKhoDuocPhamKhacExportExcelChild { get; set; }
    }

    public class XuatKhoDuocPhamKhacExportExcelChild
    {
        [Group]
        public string TenNhom { get; set; }
        [TitleGridChild("Mã")]
        public string Ma { get; set; }
        [TitleGridChild("Dược Phẩm")]
        public string Ten { get; set; }
        [TitleGridChild("ĐVT")]
        public string DVT { get; set; }
        [TitleGridChild("SL Xuất")]
        public string SoLuongXuat { get; set; }
        [TitleGridChild("Số Phiếu")]
        public string SoPhieu { get; set; }
        [TitleGridChild("Số Lô")]
        public string SoLo { get; set; }
        [TitleGridChild("Hạn Sử Dụng")]
        [Width(30)]
        public string HanSuDungDisplay { get; set; }
    }

    public class PhieuXuatKhoKhacVo
    {
        public long Id { get; set; }
        public string HostingName { get; set; }
        public bool? CoNCC { get; set; }
        public bool LaDuocPham { get; set; }
        public bool? DuocDuyet { get; set; }

    }

    public class XuatKhoDuocPhamKhacInVo
    {
        public string SoPhieu { get; set; }
        public string BarCodeImgBase64 => string.IsNullOrEmpty(SoPhieu) ? "" : BarcodeHelper.GenerateBarCode(SoPhieu, 300);
        public string SoPhieuNhap { get; set; }
        public DateTime? DTNgayNhap { get; set; }
        public string NgayNhap => DTNgayNhap?.ApplyFormatDate();
        public string SoHoaDon { get; set; }
        public DateTime? DTNgayHoaDon { get; set; }
        public string NgayHoaDon => DTNgayHoaDon?.ApplyFormatDate();

        public string NCC { get; set; }
        public string ThuocVatTu { get; set; }
        public string NgayLapPhieu => CreatedOn?.ApplyFormatDate();
        public string KhoaPhong { get; set; }
        public string KhoTraLai { get; set; }
        public string NguoiXuat { get; set; }
        public string DienGiai { get; set; }
        public int CongKhoan { get; set; }
        public string TongCong { get; set; }
        public string NgayThangNam => CreatedOn?.ApplyFormatNgayThangNam();
        public string TongTienBangChu { get; set; }
        public int ChietKhau { get; set; }
        public string VAT { get; set; }
        public string GiaTriThanhToan { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long? NhaThauId { get; set; }
    }

    public class XuatKhoDuocPhamKhacChiTietInVo
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public string HanSuDungDisplay { get; set; }
        public double SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => DonGia * Convert.ToDecimal(SoLuong);
        public string GhiChu { get; set; }
        public int VATCal { get; set; } // tính toán
        public decimal VAT => Math.Round(ThanhTien * VATCal / 100, 2);
        public decimal GiaTriThanhToan => ThanhTien + VAT;

    }

}