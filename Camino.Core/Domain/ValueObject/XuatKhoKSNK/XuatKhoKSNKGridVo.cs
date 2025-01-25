using Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.XuatKhoKSNK
{
    public class ThongTinXuatKhoKsnkVo
    {
        public ThongTinXuatKhoKsnkVo()
        {
            ThongTinXuatKhoKsnkChiTietVos = new List<ThongTinXuatKhoKsnkChiTietVo>();
        }
        public List<ThongTinXuatKhoKsnkChiTietVo> ThongTinXuatKhoKsnkChiTietVos { get; set; }
        public long KhoXuatId { get; set; }
        public long KhoNhapId { get; set; }
        public DateTime NgayXuat { get; set; }
        public long NguoiXuatId { get; set; }
        public long? NguoiNhanId { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiNhan { get; set; }
        public string TenNguoiNhan { get; set; }
        public string LyDoXuatKho { get; set; }
    }
    public class ThongTinXuatKhoKsnkChiTietVo
    {
        public string Id { get; set; }
        public double SoLuongXuat { get; set; }
        public string SoLo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)SoLuongXuat * DonGia;
        public DateTime HanSuDung { get; set; }
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public long DuocPhamVatTuId { get; set; }
        public bool LaDpVtBHYT { get; set; }
    }
    public class XuatKhoKsnkResultVo
    {
        public long? XuatKhoDuocPhamId { get; set; }
        public long? XuatKhoVatTuId { get; set; }
    }
    public class DpVtKsnkXuatGridVo : GridItem
    {
        public string Id => $"{DuocPhamVatTuId},{(int)LoaiDuocPhamVatTu},{(LaDpVtBHYT ? "1" : "0")},{HanSuDung.ToString("yyyyMMdd")},{SoLo},{DonGia}";
        public int STT { get; set; }
        public string TenVatTu { get; set; }
        public string DVT { get; set; }
        public bool LaDpVtBHYT { get; set; }
        public string Loai { get { return LaDpVtBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay
        {
            get { return SoLuongTon.ApplyNumber(); }
        }
        public double SoLuongXuat { get; set; }

        //public Enums.LoaiSuDung? LoaiSuDung { get; set; }
        public string TenNhom { get; set; }

        public string MaVatTu { get; set; }

        public string SoLo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)SoLuongXuat * DonGia;
        public DateTime HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung.ApplyFormatDate();
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public long DuocPhamVatTuId { get; set; }
    }
    public class VatTuXuatGridVo : GridItem
    {
        public string Id { get; set; }
        public int STT { get; set; }
        public string TenVatTu { get; set; }
        public string DVT { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string Loai { get { return LaVatTuBHYT ? "BHYT" : "Không BHYT"; } }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay
        {
            get { return SoLuongTon.ApplyNumber(); }
        }
        public double SoLuongXuat { get; set; }

        public Enums.LoaiSuDung? LoaiSuDung { get; set; }
        public string LoaiSuDungDisplay { get; set; }

        public string MaVatTu { get; set; }

        public string SoLo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)SoLuongXuat * DonGia;
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
    }

    public class XuatKhoKSNKGridVo : GridItem
    {
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public int STT { get; set; }
        public string KhoXuat { get; set; }
        public string KhoNhap { get; set; }
        public string SoPhieu { get; set; }

        public string LyDoXuatKho { get; set; }
        public string NguoiNhan { get; set; }
        public string NguoiXuat { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string NgayXuatDisplay
        {
            get { return NgayXuat != null ? (NgayXuat ?? DateTime.Now).ApplyFormatDateTime() : ""; }
        }
    }

    public class XuatKhoVatTuSearch
    {
        public string SearchString { get; set; }
        public RangeDates RangeXuat { get; set; }
    }

    public class XuatKhoKSNKChildrenGridVo : GridItem
    {
        public int STT { get; set; }
        public string LoaiSuDung { get; set; }
        public string VatTu { get; set; }
        public string DVT { get; set; }
        public string Loai { get; set; }
        public string SoLuongTon { get; set; }
        public string SoLuongXuat { get; set; }
        public string SoPhieu { get; set; }
    }

    public class XuatKhoDuocPhamChildrenGridVo : GridItem
    {
        public string LoaiSuDung { get; set; }
        public string DuocPham { get; set; }
        public string DVT { get; set; }
        public string Loai { get; set; }
        public string SoLuongTon { get; set; }
        public string SoLuongXuat { get; set; }
        public string SoPhieu { get; set; }
        public long? XuatKhoDuocPhamId { get; set; }
        public string SearchString { get; set; }

    }

    public class ThemKSNKNoValidate
    {
        public long? VatTuId { get; set; }
        public int? ChatLuong { get; set; }
        public double? SoLuongXuat { get; set; }
        public double? SoLuongTon { get; set; }
        public long? KhoId { get; set; }
    }

    public class ThemKSNK
    {
        public long? VatTuBenhVienId { get; set; }
        public int? ChatLuong { get; set; }
        public double? SoLuongXuat { get; set; }
        public double? SoLuongTon { get; set; }
        public long? KhoId { get; set; }

        public decimal? DonGia { get; set; }
        public int? VAT { get; set; }
        public int? ChietKhau { get; set; }

        public bool LaVatTuBHYT { get; set; }

        //public long? NhomDuocPhamId { get; set; }
        public Enums.LoaiSuDung? LoaiSuDung { get; set; }

        public Enums.EnumLoaiKhoDuocPham? loaiKhoDuocPhamXuat { get; set; }
        public Enums.XuatKhoDuocPham? loaiXuatKho { get; set; }
    }

    public class ThongTinInXuatKhoVatTuVo
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

    public class ThongTinInXuatKhoVatTuChiTietVo
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
    }

    public class XuatKhoVatTuKhacGridVo : GridItem
    {
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public string KhoVatTuXuat { get; set; }
        public string SoPhieu { get; set; }
        public string LyDoXuatKho { get; set; }
        public string TenNguoiNhan { get; set; }
        public string TenNguoiXuat { get; set; }
        public DateTime? NgayXuat { get; set; }
        public string NgayXuatDisplay => NgayXuat?.ApplyFormatDateTime();
        public bool? ChoDuyet { get; set; }
        public bool? DaDuyet { get; set; }
        public bool? DuocDuyet { get; set; }
        public int? TinhTrang => DuocDuyet == null ? 0 : 1;
        public string TinhTrangDisplay => TinhTrang == 0 ? "Chờ duyệt" : "Đã duyệt xuất";
        public string SearchString { get; set; }
        public bool? TraNCC { get; set; }
        public RangeDates RangeFromDate { get; set; }
    }

    public class YeuCauXuatKhoKSNKGridVo : GridItem
    {
        public long VatTuBenhVienId { get; set; }
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public double SoLuongTon { get; set; }
        public string SoLuongTonDisplay => SoLuongTon.ApplyNumber();
        public long? NhomVatTuId { get; set; }
        public string TenNhom { get; set; }
        public string Ma { get; set; }
        public string SoLo { get; set; }
        public double SoLuongXuat { get; set; }
        public decimal DonGiaNhap { get; set; }
        public DateTime? HanSuDung { get; set; }
        public string HanSuDungDisplay => HanSuDung?.ApplyFormatDate();
        public long? XuatKhoVatTuChiTietViTriId { get; set; }
        public long? KhoXuatId { get; set; }
        public string SoPhieu { get; set; }
        public long? XuatKhoVatTuId { get; set; }
        public string SearchString { get; set; }

        public string SoDangKy { get; set; }
        public string HamLuong { get; set; }
    }

    public class YeuCauXuatKSNKChiTietTheoKhoXuatVos
    {
        public YeuCauXuatKSNKChiTietTheoKhoXuatVos()
        {
            YeuCauXuatKhoVatTuChiTiets = new List<YeuCauXuatKhoKSNKGridVo>();
        }
        public List<YeuCauXuatKhoKSNKGridVo> YeuCauXuatKhoVatTuChiTiets { get; set; }
    }


    public class YeuCauXuatKhoVatTuChiTietVoSearch
    {
        public YeuCauXuatKhoVatTuChiTietVoSearch()
        {
            VatTuBenhViens = new List<XuatKhacVatTuBenhVienChiTietVo>();
        }
        public long? KhoXuatId { get; set; }
        public string SearchString { get; set; }
        public long? NhaThauId { get; set; }
        public string SoChungTu { get; set; }
        public Enums.LoaiDuocPhamVatTu? LoaiDuocPhamVatTu { get; set; }
        public List<XuatKhacVatTuBenhVienChiTietVo> VatTuBenhViens { get; set; }
    }

    public class XuatKhacVatTuBenhVienChiTietVo
    {
        public long VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }

    }

    public class XuatKhoKhacKSNKVo
    {
        public long Id { get; set; }
        public long? KhoXuatId { get; set; }
        public long? NguoiXuatId { get; set; }
        public string TenNguoiXuat { get; set; }
        public long? NguoiNhanId { get; set; }
        public string TenNguoiNhan { get; set; }
        public bool? TraNCC { get; set; }
        public string TenNhaThau { get; set; }
        public long? NhaThauId { get; set; }
        public string SoChungTu { get; set; }
        public string LyDoXuatKho { get; set; }
        public DateTime? NgayXuat { get; set; }
        public Enums.LoaiDuocPhamVatTu? LoaiDuocPhamVatTu { get; set; }
    }

    public class XuatKhoKhacKSNKResultVo
    {
        public XuatKhoKhacKSNKResultVo()
        {
            PhieuInDuocPhamVaVatTus = new List<string>();
        }
        public long? XuatDuocPhamId { get; set; }
        public long? XuatVatTuId { get; set; }

        public List<string> PhieuInDuocPhamVaVatTus { get; set; }
    }
    public class CapNhatXuatKhoKhacKSNKResultVo
    {
        public long Id { get; set; }
        public byte[] LastModified { get; set; }
    }

    public class XuatKhoKhacKSNKChiTietVo
    {
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public double? SoLuongXuat { get; set; }
        public string Ma { get; set; }
        public long VatTuBenhVienId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
        public long? KhoXuatId { get; set; }
        public string SoLo { get; set; }
        public DateTime? HanSuDung { get; set; }
    }

    public class XuatKhoKhacVatTuRsVo : GridItem
    {
        public virtual byte[] LastModified { get; set; }
    }


    public class XuatKhoVatTuKhacExportExcel : GridItem
    {
        public XuatKhoVatTuKhacExportExcel()
        {
            XuatKhoVatTuKhacExportExcelChild = new List<XuatKhoVatTuKhacExportExcelChild>();
        }
        [Width(30)]
        public string SoPhieu { get; set; }
        [Width(30)]
        public string KhoVatTuXuat { get; set; }
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
        public List<XuatKhoVatTuKhacExportExcelChild> XuatKhoVatTuKhacExportExcelChild { get; set; }
    }

    public class XuatKhoVatTuKhacExportExcelChild
    {
        [Group]
        public string TenNhom { get; set; }
        [TitleGridChild("Mã")]
        public string Ma { get; set; }
        [TitleGridChild("Vật Tư")]
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


    public class XuatKhoKhacKSNKExportExcel : GridItem
    {
        public XuatKhoKhacKSNKExportExcel()
        {
            XuatKhoKhacKSNKExportExcelChild = new List<XuatKhoKhacKSNKExportExcelChild>();
        }
        [Width(30)]
        public string SoPhieu { get; set; }
        [Width(30)]
        public string KhoVatTuXuat { get; set; }
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

        public LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }

        public List<XuatKhoKhacKSNKExportExcelChild> XuatKhoKhacKSNKExportExcelChild { get; set; }
    }

    public class XuatKhoKhacKSNKExportExcelChild
    {
        [Group]
        public string TenNhom { get; set; }
        [TitleGridChild("Mã")]
        public string Ma { get; set; }
        [TitleGridChild("Vật Tư")]
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

    public class SoCTTheoKhoDuocPhamJsonVo
    {
        public long? Id { get; set; }
        public long? NhaThauId { get; set; }
        public long? KhoId { get; set; }
    }

    public class XuatKhoKhacLookupItem : LookupItemVo
    {
        public EnumLoaiKhoDuocPham LoaiKho { get; set; }
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

    public class PhieuXuatKhoKhacVo
    {
        public long Id { get; set; }
        public string HostingName { get; set; }
        public bool? CoNCC { get; set; }
        public bool LaDuocPham { get; set; }
        public bool? DuocDuyet { get; set; }

    }

    public class XuatKhoKhacKSNKInVo
    {
        public string SoPhieu { get; set; }
        public string BarCodeImgBase64 => string.IsNullOrEmpty(SoPhieu) ? "" : BarcodeHelper.GenerateBarCode(SoPhieu, 300);
        public string SoPhieuNhap { get; set; }
        public DateTime? DTNgayNhap { get; set; }
        public string NgayNhap => DTNgayNhap?.ApplyFormatDate();
        public string SoHoaDon { get; set; }
        public DateTime? DTNgayHoaDon { get; set; }
        public string NgayHoaDon => DTNgayHoaDon?.ApplyFormatDate();
        public string Header { get; set; }

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

    public class XuatKhoKhacKSNKKhacChiTietInVo
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
