using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.KhoVatTus
{
    public class ThongTinHoanTraVatTu
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
    }

    public class DanhSachHoanTraVatTuVo : GridItem
    {
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

    public class HoanTraVatTuSearch
    {
        public bool DangChoDuyet { get; set; }
        public bool TuChoiDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeNhap { get; set; }
        public RangeDate RangeDuyet { get; set; }
    }    

    public class DanhSachHoanTraVatTuChiTietVo : GridItem
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

    public class DuyetHoanTraVatTuExportExcel : GridItem
    {
        public DuyetHoanTraVatTuExportExcel()
        {
            DuyetHoanTraVatTuExportExcelChild = new List<DuyetHoanTraVatTuExportExcelChild>();
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


        public List<DuyetHoanTraVatTuExportExcelChild> DuyetHoanTraVatTuExportExcelChild { get; set; }
    }

    public class DuyetHoanTraVatTuExportExcelChild
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
        public string HanSuDungStr { get; set; }
        [TitleGridChild("Số Lượng Hoàn Trả")]
        public double SoLuongHoanTra { get; set; }
    }

    //public class HoanTraVatTuExportExcel : GridItem
    //{
    //    public HoanTraVatTuExportExcel()
    //    {
    //        HoanTraVatTuExportExcelChild = new List<HoanTraVatTuExportExcelChild>();
    //    }

    //    public string SoChungTu { get; set; }
    //    public string TenNguoiNhap { get; set; }
    //    public string NguoiGiao { get; set; }
    //    public string NgayNhap { get; set; }
    //    public bool? TinhTrang { get; set; }
    //    public string TinhTrangDuyet => TinhTrang == null ? "Đang chờ duyệt" : TinhTrang == true ? "Đã duyệt" : "Từ chối duyệt";
    //    public string ClassTrangThai => TinhTrang == null ? "dang_cho_duyet" : TinhTrang == true ? "da_duyet" : "tu_choi_duyet";
    //    public string NguoiDuyet { get; set; }
    //    public string NgayDuyet { get; set; }


    //    public List<HoanTraVatTuExportExcelChild> HoanTraVatTuExportExcelChild { get; set; }
    //}

    //public class  HoanTraVatTuExportExcelChild
    //{
    //    [TitleGridChild("Tên Vật Tư")]
    //    public string VatTu { get; set; }
    //    [TitleGridChild("Hợp Đồng Thầu")]
    //    public string HopDongThau { get; set; }
    //    [TitleGridChild("Loại BHYT")]
    //    public string LoaiBHYTDisplay { get; set; }  
    //    [TitleGridChild("Số Lô")]
    //    public string SoLo { get; set; }
    //    [TitleGridChild("Hạn Sử Dụng")]
    //    public string HanSuDung { get; set; }
    //    [TitleGridChild("Mã Vạch")]
    //    public string MaVach { get; set; }
    //    [TitleGridChild("Số Lượng Nhập")]
    //    public double SoLuongNhap { get; set; }
    //    [TitleGridChild("Đơn Giá Nhập")]
    //    public decimal DonGiaNhap { get; set; }
    //    [TitleGridChild("VAT(%)")]
    //    public string VAT { get; set; }
    //    [TitleGridChild("TL BH THANH TOÁN(%)")]
    //    public string TiLeBHYTThanhToan { get; set; }
    //    [TitleGridChild("GIÁ BÁN(%)")]
    //    public string GiaBan { get; set; }
    //}

    public class PhieuHoanTraVatTuData
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
    public class PhieuHoanTraVatTuChiTietData
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
}
