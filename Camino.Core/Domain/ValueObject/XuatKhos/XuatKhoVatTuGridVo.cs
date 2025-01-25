using Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.XuatKhos
{
    public class VatTuXuatGridVo : GridItem
    {
        //public string Id { get; set; }
        public string Id => $"{VatTuBenhVienId},{(LaVatTuBHYT ? "1" : "0")},{HanSuDung?.ToString("yyyyMMdd")},{SoLo},{DonGia}";
        public long VatTuBenhVienId { get; set; }
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

    public class XuatKhoVatTuGridVo : GridItem
    {
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


    public class ThongTinXuatKhoVatTuVo
    {
        public ThongTinXuatKhoVatTuVo()
        {
            ThongTinXuatKhoVatTuChiTietVos = new List<ThongTinXuatKhoVatTuChiTietVo>();
        }
        public List<ThongTinXuatKhoVatTuChiTietVo> ThongTinXuatKhoVatTuChiTietVos { get; set; }
        public long KhoXuatId { get; set; }
        public long KhoNhapId { get; set; }
        public DateTime NgayXuat { get; set; }
        public long NguoiXuatId { get; set; }
        public long? NguoiNhanId { get; set; }
        public Enums.LoaiNguoiGiaoNhan LoaiNguoiNhan { get; set; }
        public string TenNguoiNhan { get; set; }
        public string LyDoXuatKho { get; set; }
    }
    public class ThongTinXuatKhoVatTuChiTietVo
    {
        public string Id { get; set; }
        public double SoLuongXuat { get; set; }
        public string SoLo { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => (decimal)SoLuongXuat * DonGia;
        public DateTime HanSuDung { get; set; }
        public long VatTuId { get; set; }
        public bool LaVTBHYT { get; set; }
    }

    public class XuatKhoVatTuSearch
    {
        public string SearchString { get; set; }
        public RangeDates RangeXuat { get; set; }
    }

    public class XuatKhoVatTuChildrenGridVo : GridItem
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

    public class ThemVatTuNoValidate
    {
        public long? VatTuId { get; set; }
        public int? ChatLuong { get; set; }
        public double? SoLuongXuat { get; set; }
        public double? SoLuongTon { get; set; }
        public long? KhoId { get; set; }
    }

    public class ThemVatTu
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
        public bool? CoNCC { get; set; }
        public RangeDates RangeFromDate { get; set; }
    }

    public class YeuCauXuatKhoVatTuGridVo : GridItem
    {
        public long VatTuBenhVienId { get; set; }
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

    }

    public class YeuCauXuatVatTuChiTietTheoKhoXuatVos
    {
        public YeuCauXuatVatTuChiTietTheoKhoXuatVos()
        {
            YeuCauXuatKhoVatTuChiTiets = new List<YeuCauXuatKhoVatTuGridVo>();
        }
        public List<YeuCauXuatKhoVatTuGridVo> YeuCauXuatKhoVatTuChiTiets { get; set; }
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

    public class XuatKhoKhacVatTuChiTietVo
    {
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
}
