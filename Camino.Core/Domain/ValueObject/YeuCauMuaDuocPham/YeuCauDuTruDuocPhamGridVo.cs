using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham
{
    public class YeuCauDuTruDuocPhamGridVo : GridItem
    {
        public string SoPhieu { get; set; }
        public string TenNhomDuTru { get; set; }
        public EnumNhomDuocPhamDuTru NhomDuocPhamDuTru { get; set; }
        public string TenKho { get; set; }
        public DateTime? TuNgay { get; set; }
        public string KyDuTru { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay => NgayYeuCau?.ApplyFormatDateTimeSACH();
        public string NhanVienYeuCau { get; set; }
        // 0: chờ duyệt, 1: đã duyệt, 2: từ chối
        public int? TinhTrang => DaDuyet == true ? 1 : (TuChoiDuyet == true ? 2 : 0);
        public DateTime? NgayTaiKhoa { get; set; }
        public string NgayTaiKhoaDisplay => NgayTaiKhoa?.ApplyFormatDateTimeSACH();
        public DateTime? NgayTaiKhoDuoc { get; set; }
        public string NgayTaiKhoDuocDisplay => NgayTaiKhoDuoc?.ApplyFormatDateTimeSACH();
        public DateTime? NgayTaiGiamDoc { get; set; }
        public string NgayTaiGiamDocDisplay => NgayTaiGiamDoc?.ApplyFormatDateTimeSACH();
        public bool? ChoDuyet { get; set; }
        public bool? DaDuyet { get; set; }
        public bool? TuChoiDuyet { get; set; }
        public string NgayYeuCauTu { get; set; }
        public string NgayYeuCauDen { get; set; }
        public string SearchString { get; set; }
        public bool? IsKhoaDuyet { get; set; }
        public RangeDates RangeFromDate { get; set; }
    }

    public class RangeDates
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }

    public class ThongTinDuTruMuaDuocPham
    {
        public ThongTinDuTruMuaDuocPham()
        {
            ThongTinChiTietTonKhoTongs = new List<ThongTinChiTietTonKho>();
            ThongTinChiTietTonHDTs = new List<ThongTinChiTietTonKho>();
        }
        public string HoatChat { get; set; }
        public string SoDangKy { get; set; }
        public string HamLuong { get; set; }
        public string DVT { get; set; }
        public string TenDuongDung { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public double? SLTonDuTru { get; set; }
        public string SoLuongTonDuTru => SLTonDuTru.GetValueOrDefault().ApplyNumber();
        public double? SLTonKhoTong { get; set; }
        public string SoLuongTonKhoTong => SLTonKhoTong.GetValueOrDefault().ApplyNumber();
        public double? SLChuaNhapVeHDT { get; set; }
        public string SoLuongChuaNhapVeHDT => SLChuaNhapVeHDT.GetValueOrDefault().ApplyNumber();
        public List<ThongTinChiTietTonKho> ThongTinChiTietTonKhoTongs { get; set; }
        public List<ThongTinChiTietTonKho> ThongTinChiTietTonHDTs { get; set; }
    }

    public class ThongTinChiTietTonKho
    {
        public string Ten { get; set; }
        public double? SLTon { get; set; }
        public string SoLuongTon => SLTon.GetValueOrDefault().ApplyNumber();
    }

    public class ThongTinChiTietDuocPhamTonKho
    {
        public long DuocPhamId { get; set; }
        public long KhoId { get; set; }
        public int LoaiDuocPham { get; set; }
        //public long HopDongThauDuocPhamId { get; set; }
    }

    public class PhieuMuaDuTruDuocPhamData
    {
        public string Header { get; set; }
        public string MaPhieuMuaDuTruHoaChat { get; set; }
        public string MaPhieuMuaDuTruDuocPham { get; set; }
        public string Thuoc { get; set; } // content
        public string KhoaPhong { get; set; }
        public string TenKho { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string NhanVienLap { get; set; }

    }
    public class PhieuMuaDuTruDuocPhamChiTietData
    {
        public string MaHang { get; set; }
        public string Ten { get; set; }
        public string HamLuong { get; set; }
        public string HoatChat { get; set; }
        public string DonVi { get; set; }
        public int SoLuong { get; set; }
        public string Nhom { get; set; }
        public string GhiChu { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public long DuocPhamId { get; set; }

    }

    public class DuocPhamTemplateLookupItem
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ten { get; set; }
        public string HoatChat { get; set; }
        public string DVT { get; set; }
        public string TenDuongDung { get; set; }
        public string SoDangKy { get; set; }
        public string HamLuong { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public int? Rank { get; set; }
    }
    public class DuocPhamVaHoaChat
    {
        public long DuocPhamHoaChatId { get; set; }
    }
    public class DuocPhamDuTruViewModelValidator
    {
        public long? DuocPhamId { get; set; }
        public bool? LaDuocPhamBHYT { get; set; }
    }
    public class KyDuTruMuaDuocPhamVatTuVo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

    }

    public class PhieuMuaDuTruDuocPham
    {
        public long DuTruMuaDuocPhamId { get; set; }
        public string HostingName { get; set; }
        public bool? Header { get; set; }
        public bool? TrangThai { get; set; }
    }


    public class YeuCauDuTruDuocPhamExcel
    {
        [Width(20)]
        public string SoPhieu { get; set; }
        [Width(20)]
        public string TenNhomDuTru { get; set; }
        [Width(20)]
        public string TenKho { get; set; }
        [Width(40)]
        public string KyDuTru { get; set; }
        [Width(40)]
        public string NgayYeuCauDisplay { get; set; }
        [Width(20)]
        public string NhanVienYeuCau { get; set; }
        [Width(20)]
        public string TinhTrang { get; set; }
        [Width(40)]
        public string NgayTaiKhoaDisplay { get; set; }
        [Width(40)]
        public string NgayTaiKhoDuocDisplay { get; set; }
        [Width(40)]
        public string NgayTaiGiamDocDisplay { get; set; }

    }
}
