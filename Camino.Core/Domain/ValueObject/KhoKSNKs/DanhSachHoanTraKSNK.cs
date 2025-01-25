using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.KhoKSNKs
{
    public class ThongTinHoanTraKSNK
    {
        public string SoChungTu { get; set; }
        public long NguoiYeuCauId { get; set; }
        public string TenNguoiYeuCau { get; set; }
        public string GhiChu { get; set; }

        public long HoanTraTuKhoId { get; set; }
        public string HoanTraTuKho { get; set; }
        public long HoanTraVeKhoId { get; set; }
        public string HoanTraVeKho { get; set; }
       
        public DateTime? NgayYeuCau { get; set; }
        public bool? TinhTrang { get; set; }
        public string TinhTrangDuyet => TinhTrang == null ? "Đang chờ duyệt" : TinhTrang == true ? "Đã duyệt" : "Từ chối duyệt";
        public string ClassTrangThai => TinhTrang == null ? "dang_cho_duyet" : TinhTrang == true ? "da_duyet" : "tu_choi_duyet";

        public long? NguoiDuyetId { get; set; }
        public string NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }

        public long? NguoiTraId { get; set; }
        public string TenNguoiTra { get; set; }
        public long? NguoiNhanId { get; set; }
        public string TenNguoiNhan { get; set; }
        public string LyDoHuy { get; set; }

        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
    }

    public class DanhSachHoanTraKSNKVo : GridItem
    {
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public string SoPhieu { get; set; }
        public long NguoiYeuCauId { get; set; }
        public string TenNguoiYeuCau { get; set; }
        public string GhiChu { get; set; }

        public long HoanTraTuKhoId { get; set; }
        public string HoanTraTuKho { get; set; }
        public long HoanTraVeKhoId { get; set; }
        public string HoanTraVeKho { get; set; }

        public string NgayYeuCauDisplay { get; set; }
        public bool? TinhTrang { get; set; }

        public string TinhTrangDuyet => TinhTrang == null ? "Đang chờ duyệt" : TinhTrang == true ? "Đã duyệt" : "Từ chối duyệt";
        public string ClassTrangThai => TinhTrang == null ? "dang_cho_duyet" : TinhTrang == true ? "da_duyet" : "tu_choi_duyet";

        public string NguoiDuyet { get; set; }
        public string NgayDuyetDisplay { get; set; }

        public DateTime NgayYeuCau { get; set; }
        public DateTime? NgayDuyet { get; set; }

    }

    public class HoanTraKSNKSearch
    {
        public bool DangChoDuyet { get; set; }
        public bool TuChoiDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeNhap { get; set; }
        public RangeDate RangeDuyet { get; set; }
    }    

    public class DanhSachHoanTraKSNKChiTietVo : GridItem
    {
        public string LoaiSuDung { get; set; }

        public string Ma { get; set; }
        public string VatTu { get; set; }
        public string DonViTinh { get; set; }
        public bool LoaiBHYT { get; set; }
        public string LoaiBHYTDisplay => LoaiBHYT ? "BHYT" : "Không BHYT";
        public string SoLo { get; set; }          
        public DateTime HanSuDung { get; set; }
        public string HanSuDungStr => HanSuDung.ApplyFormatDate();
        public double SoLuongHoanTra { get; set; }
        public string SoLuongHoanTraDisplay { get; set; }
    }

    public class DuyetHoanTraKSNKExportExcel : GridItem
    {
        public DuyetHoanTraKSNKExportExcel()
        {
            DuyetHoanTraKSNKExportExcelChild = new List<DuyetHoanTraKSNKExportExcelChild>();
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


        public List<DuyetHoanTraKSNKExportExcelChild> DuyetHoanTraKSNKExportExcelChild { get; set; }
    }

    public class DuyetHoanTraKSNKExportExcelChild
    {
        [TitleGridChild("Mã")]
        public string Ma { get; set; }
        [TitleGridChild("Tên")]
        public string VatTu { get; set; }
        [TitleGridChild("ĐVT")]
        public string DonViTinh { get; set; }
        [TitleGridChild("Loại BHYT")]
        public string LoaiBHYTDisplay { get; set; }
        [TitleGridChild("Số Lô")]
        public string SoLo { get; set; }
        [TitleGridChild("Hạn Sử Dụng")]
        public string HanSuDungStr { get; set; }
        [TitleGridChild("Số Lượng Hoàn Trả")]
        public double SoLuongHoanTra { get; set; }
    }   

    public class PhieuHoanTraKSNKData
    {
        public string LogoUrl { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string KhoTraLai { get; set; }
        public string KhoNhan { get; set; }
        public string ThuocVatTu { get; set; } // content
        public string NgayLapPhieu { get; set; }
        public string SoPhieu { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }

    }
    public class PhieuHoanTraKSNKChiTietData
    {
        public string Ten { get; set; }
        public string NuocSX { get; set; }
        public string SoLo { get; set; }
        public DateTime HanSuDung { get; set; }
        public string TenVatTu => Ten + " "  + (!string.IsNullOrEmpty(NuocSX) ? "(" + NuocSX + ")" : " ") + (!string.IsNullOrEmpty(SoLo) ? SoLo : " ") + " - " + HanSuDung.ApplyFormatDate();
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


    public class DanhSachYeuCauHoanTraKSNKGridVo : GridItem
    {
        public Enums.LoaiDuocPhamVatTu LoaiDuocPhamVatTu { get; set; }
        public string Ma { get; set; }

        public string NguoiYeuCau { get; set; }

        public string KhoHoanTraTu { get; set; }

        public string KhoHoanTraVe { get; set; }

        public DateTime? NgayYeuCau { get; set; }

        public string NgayYeuCauDisplay => NgayYeuCau != null ? NgayYeuCau.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        public bool? TinhTrang { get; set; }

        public string TinhTrangDisplay => GetTinhTrang(TinhTrang);

        public string NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }

        public string NgayDuyetDisplay => NgayDuyet != null ? NgayDuyet.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        private string GetTinhTrang(bool? tinhTrang)
        {
            if (tinhTrang == null) return "Đang chờ duyệt";

            return tinhTrang != false ? "Đã duyệt" : "Từ chối duyệt";
        }
    }

    public class DanhSachYCHoanTraKSNKChiTietGridVo : GridItem
    {
        public string VatTu { get; set; }
        public string Ma { get; set; }
        public string DVT { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string HopDong { get; set; }

        public bool LaVatTuBhyt { get; set; }

        public string Loai => GetLoaiBhyt(LaVatTuBhyt);

        private string GetLoaiBhyt(bool laBhyt)
        {
            return laBhyt ? "BHYT" : "Không BHYT";
        }

        public string SoLo { get; set; }

        public DateTime HanSuDung { get; set; }

        public DateTime NgayNhapVaoBenhVien { get; set; }

        public string HsdText => HanSuDung.ApplyFormatDate();

        public string NgayNhapBvText => NgayNhapVaoBenhVien.ApplyFormatDate();

        public decimal DonGiaNhap { get; set; }

        public int TiLeThapGia { get; set; }

        public int Vat { get; set; }

        public string MaVach { get; set; }

        public string Nhom { get; set; }

        public double SoLuongTra { get; set; }
    }
}
