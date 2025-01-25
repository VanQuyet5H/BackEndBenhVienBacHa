using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.KhoVatTus
{

    public class ThongTinLyDoHuyNhapKhoVatTu
    {
        public long YeuCauNhapKhoVatTuId { get; set; }
        public string LyDoHuy { get; set; }
    }

    public class ThongTinDuyetKhoVatTu
    {
        public string SoChungTu { get; set; }
        public long NguoiNhapId { get; set; }
        public string TenNguoiNhap { get; set; }
        public DateTime? NgayNhap { get; set; }
        public bool? TinhTrang { get; set; }
        public string TinhTrangDuyet => TinhTrang == null ? "Đang chờ duyệt" : TinhTrang == true ? "Đã duyệt" : "Từ chối duyệt";
        public string ClassTrangThai => TinhTrang == null ? "dang_cho_duyet" : TinhTrang == true ? "da_duyet" : "tu_choi_duyet";

        public long? NguoiDuyetId { get; set; }
        public string NguoiDuyet { get; set; }
        public DateTime? NgayDuyet { get; set; }
    }

    public class DanhSachDuyetKhoVatTuVo : GridItem
    {
        public string SoChungTu { get; set; }
        public long NguoiNhapId { get; set; }
        public string TenNguoiNhap { get; set; }
        public string NguoiGiao { get; set; }
       
        public bool? TinhTrang { get; set; }
        public string TinhTrangDuyet => TinhTrang == null ? "Đang chờ duyệt" : TinhTrang == true ? "Đã duyệt" : "Từ chối duyệt";
        public string ClassTrangThai => TinhTrang == null ? "dang_cho_duyet" : TinhTrang == true ? "da_duyet" : "tu_choi_duyet";
        public int TinhTrangNumber => TinhTrang == null ? 0 : TinhTrang == true ? 1 : 2;
        public string NguoiDuyet { get; set; }

        public string NgayNhapDisplay => NgayNhap?.ApplyFormatDateTime();
        public string NgayDuyetDisplay => NgayDuyet?.ApplyFormatDateTime();
        public DateTime? NgayNhap { get; set; }
        public DateTime? NgayDuyet { get; set; }
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay => NgayHoaDon?.ApplyFormatDateTime();

        public string TenKho { get; set; }
        public bool? DangChoDuyet { get; set; }
        public bool? TuChoiDuyet { get; set; }
        public bool? DaDuyet { get; set; }
        public bool? DuyetLai { get; set; }
        public string SearchString { get; set; }
        public RangeDates RangeNhap { get; set; }
        public RangeDates RangeDuyet { get; set; }


        //BVHD-3926
        public RangeDates RangeHoaDon { get; set; }
        public string TenNhaCungCap { get; set; }
    }

    public class NhapKhoVatTuSearch
    {
        public bool DangChoDuyet { get; set; }
        public bool TuChoiDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeNhap { get; set; }
        public RangeDate RangeDuyet { get; set; }

        //BVHD-3926
        public RangeDate RangeHoaDon { get; set; }
    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class DanhSachDuyetKhoVatTuChiTietVo : GridItem
    {
        public string VatTu { get; set; }
        public string NhaCungCap { get; set; }
        public string HopDongThau { get; set; }
        public bool LoaiBHYT { get; set; }
        public string LoaiBHYTDisplay => LoaiBHYT ? "Có" : "Không";
        public string NhomDanhSachThuoc { get; set; }
        public string SoLo { get; set; }
        public string HanSuDung { get; set; }
        public string MaVach { get; set; }
        public string SLConLaiHD { get; set; }
        public string SoLuongNhap { get; set; }
        public  int? TiLeBHYTThanhToan { get; set; }         
        public string VAT { get; set; }
        public decimal DonGiaNhap { get; set; }
        public decimal GiaBan { get; set; }
        public string LoaiSuDung { get; set; }

        public string NgayNhapDisplay { get; set; }
        public string NgayDuyetDisplay { get; set; }
    }



    public class DuyetVatTuExportExcel : GridItem
    {
        public DuyetVatTuExportExcel()
        {
            DuyetVatTuExportExcelChild = new List<DuyetVatTuExportExcelChild>();
        }

        public string SoChungTu { get; set; }
        public string TenNguoiNhap { get; set; }
        public string NguoiGiao { get; set; }
        public string NgayNhap { get; set; }
        public bool? TinhTrang { get; set; }
        public string TinhTrangDuyet => TinhTrang == null ? "Đang chờ duyệt" : TinhTrang == true ? "Đã duyệt" : "Từ chối duyệt";
        public string ClassTrangThai => TinhTrang == null ? "dang_cho_duyet" : TinhTrang == true ? "da_duyet" : "tu_choi_duyet";
        public string NguoiDuyet { get; set; }
        public string NgayDuyet { get; set; }

        public string NgayNhapDisplay { get; set; }
        public string NgayDuyetDisplay { get; set; }
        public string NgayHoaDonDisplay { get; set; }
        public string TenKho { get; set; }


        public List<DuyetVatTuExportExcelChild> DuyetVatTuExportExcelChild { get; set; }
    }

    public class DuyetVatTuExportExcelChild
    {
        [TitleGridChild("Tên Vật Tư")]
        public string VatTu { get; set; }
        [TitleGridChild("Hợp Đồng Thầu")]
        public string HopDongThau { get; set; }
        [TitleGridChild("Loại BHYT")]
        public string LoaiBHYTDisplay { get; set; }  
        [TitleGridChild("Số Lô")]
        public string SoLo { get; set; }
        [TitleGridChild("Hạn Sử Dụng")]
        public string HanSuDung { get; set; }
        [TitleGridChild("Mã Vạch")]
        public string MaVach { get; set; }
        [TitleGridChild("Số Lượng Nhập")]
     
        public string SoLuongNhap { get; set; }
        [TitleGridChild("Đơn Giá Nhập")]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string DonGiaNhap { get; set; }
        [TitleGridChild("VAT(%)")]     
        public string VAT { get; set; }
        [TitleGridChild("TL BH THANH TOÁN(%)")]      
        public string TiLeBHYTThanhToan { get; set; }
        [TitleGridChild("GIÁ BÁN")]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public string GiaBan { get; set; }
    }
}
