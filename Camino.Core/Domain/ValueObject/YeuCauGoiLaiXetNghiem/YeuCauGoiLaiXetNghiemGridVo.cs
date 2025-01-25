using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.XetNghiem
{
    public class ThongTinHanhChinhXN
    {
        public string MaBarCode { get; set; }
        public string MaBN { get; set; }
        public string MaTN { get; set; }
        public string HoTen { get; set; }
        public int? Tuoi { get; set; }
        public string GioiTinh { get; set; }
        public string DungTuyen { get; set; }
        public int? MucHuong { get; set; }
        public string DanToc { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string MaBhyt { get; set; }
        //chuẩn đoán , khoa chỉ định , phòng khám cách nhau ";"
        public string ChuanDoan { get; set; }
        public string KhoaChiDinh { get; set; }
        public string PhongKham { get; set; }

        public bool TrangThai { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }
    }

    public class YeuCauGoiLaiXetNghiemGridVo : GridItem
    {
        public string MaBarCode { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }

        public int TrangThai { get; set; }
        public string TenTrangThai { get; set; }
        public string NguoiDuyetKetQua { get; set; }
        public long PhienXetNghiemId { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public DateTime? NgayDuyetKetQua { get; set; }
        public List<long> DanhSachPhienXetNghiemIds { get; set; }
        public string NgayDuyetKetQuaDisplay => NgayDuyetKetQua?.ApplyFormatDateTimeSACH();
    }

    public class YeuCauGoiLaiXetNghiemChiTietGridVo : GridItem
    {
        public string NhomXetNghiem { get; set; }
        public string NguoiYeuCau { get; set; }

        public DateTime NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay => NgayYeuCau.ApplyFormatDateTimeSACH();
        public string LyDoYeuCau { get; set; }


        public int TrangThai { get; set; }
        public string TenTrangThai { get; set; }
        public string NguoiDuyet { get; set; }

        public DateTime? NgayDuyet { get; set; }
        public string NgayDuyetDisplay => NgayDuyet?.ApplyFormatDateTimeSACH();

        public string LyDoTuChoi { get; set; }
    }

    public class YeuCauGoiLaiXetNghiemSearch
    {
        public bool DangChoDuyet { get; set; }
        public bool TuChoiDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public string SearchString { get; set; }
        public DuyetYeuCauChayLaiXetNghiemTimKiemDateRange TuNgayDenNgay { get; set; }
    }
    public class DuyetYeuCauChayLaiXetNghiemTimKiemDateRange
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }

        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }

    public class TuChoiYeuCauGoiLaiXetNghiem
    {
        public long PhienXetNghiemId { get; set; }
        public string LyDoTuChoi { get; set; }
    }
    public class DanhSachGoiXetNghiemLai
    {
        public List<DuyetYeuCauGoiLaiXetNghiem> DuyetYeuCauGoiLaiXetNghiems { get; set; }
    }
    public class DuyetYeuCauGoiLaiXetNghiem
    {
        public long Id { get; set; }
        public long NhanVienYeuCauId { get; set; }
    }


    public class YeuCauXetNghiemChayLaiExportExcel : GridItem
    {
        public YeuCauXetNghiemChayLaiExportExcel()
        {
            YeuCauXetNghiemChayLaiExportExcelChild = new List<YeuCauXetNghiemChayLaiExportExcelChild>();
        }
        public string MaBarCode { get; set; }
        public string MaTN { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }

        public int TrangThai { get; set; }
        public string TenTrangThai { get; set; }
        public string NguoiDuyetKetQua { get; set; }
        public long PhienXetNghiemId { get; set; }
        public DateTime? NgayDuyetKetQua { get; set; }
        public string NgayDuyetKetQuaDisplay => NgayDuyetKetQua?.ApplyFormatDateTimeSACH();
        public List<YeuCauXetNghiemChayLaiExportExcelChild> YeuCauXetNghiemChayLaiExportExcelChild { get; set; }
    }

    public class YeuCauXetNghiemChayLaiExportExcelChild
    {
        [TitleGridChild("Nhóm XN")]
        public string NhomXetNghiem { get; set; }
        [TitleGridChild("Người yêu cầu")]
        public string NguoiYeuCau { get; set; }
        [TitleGridChild("Ngày yêu cầu")]
        public string NgayYeuCauDisplay { get; set; }
        [TitleGridChild("Lý do yêu cầu")]
        public string LyDoYeuCau { get; set; }
        [TitleGridChild("Tình trạng")]
        public string TenTrangThai { get; set; }
        [TitleGridChild("Người duyệt")]
        public string NguoiDuyet { get; set; }
        [TitleGridChild("Ngày duyệt")]
        public string NgayDuyetDisplay { get; set; }
        [TitleGridChild("Lý do")]
        public string LyDoTuChoi { get; set; }
    }

    public class KetQuaXetNghiemChiTietVo : GridItem
    {
        public KetQuaXetNghiemChiTietVo()
        {
            datas = new List<ListDataChild>();
        }
        public long PhienXetNghiemId { get; set; }
        public string Barcode { get; set; }
        public List<ListDataChild> datas { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public bool? YeuCauChayLai { get; set; }
        public string TenTrangThai { get; set; }
        public int TrangThai { get; set; }
        public long NhanVienYeuCauId { get; set; }
        public string NguoiYeuCau { get; set; }
        public string NgayYeuCauDisplay { get; set; }
        public string LyDoYeuCau { get; set; }
        public string NguoiDuyet { get; set; }
        public string NgayDuyetDisplay { get; set; }
        public List<string> DanhSachLoaiMauDaCoKetQua { get; set; }
        public List<string> DanhSachLoaiMau { get; set; }
        public int LanThucHien { get; set; }
        public string LyDo { get; set; }
    }

    public class ListDataChild
    {
        public long Id { get; set; }
        public ListDataChild()
        {
            Items = new List<ListDataChild>();
        }
        //1 bình thường, 2 bất thường, 3 nguy hiểm
        public int LoaiKetQuaTuMay { get; set; } = 1;

        public string Ten { get; set; }
        public string GiaTriCu { get; set; }
        public string GiaTriTuMay { get; set; }
        public string GiaTriNhapTay { get; set; }
        public string GiaTriDuyet { get; set; }
        public bool? ToDamGiaTri { get; set; }
        public string CSBT { get; set; }
        public string DonVi { get; set; }
        public DateTime? ThoiDiemGuiYeuCau { get; set; }
        public string ThoiDiemGuiYeuCauDisplay { get { return ThoiDiemGuiYeuCau != null ? (ThoiDiemGuiYeuCau ?? DateTime.Now).ApplyFormatDateTime() : ""; } }
        public DateTime? ThoiDiemNhanKetQua { get; set; }
        public string ThoiDiemNhanKetQuaDisplay { get { return ThoiDiemNhanKetQua != null ? (ThoiDiemNhanKetQua ?? DateTime.Now).ApplyFormatDateTime() : ""; } }

        public long? MayXetNghiemId { get; set; }

        public DateTime? ThoiDiemDuyetKetQua { get; set; }
        public string ThoiDiemDuyetKetQuaDisplay { get { return ThoiDiemDuyetKetQua != null ? (ThoiDiemDuyetKetQua ?? DateTime.Now).ApplyFormatDateTime() : ""; } }
        public string NguoiDuyet { get; set; }
        public List<string> DanhSachLoaiMau { get; set; }

        public bool IsRoot { get; set; }
        public bool IsParent { get; set; }
        public bool Expanded { get; set; } = true;
        public string NgayDuyet { get { return ThoiDiemDuyetKetQua != null ? (ThoiDiemDuyetKetQua ?? DateTime.Now).ApplyFormatDateTime() : null; } }

        public List<ListDataChild> Items { get; set; }
        public string LoaiMau { get; set; }
        public long DichVuXetNghiemId { get; set; }
        public string TenMayXetNghiem { get; set; }
    }
}