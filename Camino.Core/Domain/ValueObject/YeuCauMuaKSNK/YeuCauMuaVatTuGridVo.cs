using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using Camino.Core.Helpers;
using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using System.ComponentModel;

namespace Camino.Core.Domain.ValueObject.YeuCauMuaKSNK
{
    public class YeuCauMuaKSNKGridVo : GridItem
    {
        public string SoPhieu { get; set; }
        public string TenNhomDuTru { get; set; }
        public string TenKho { get; set; }
        public DateTime? TuNgay { get; set; }
        public string KyDuTru { get; set; }
        public DateTime? NgayYeuCau { get; set; }
        public string NgayYeuCauDisplay => NgayYeuCau?.ApplyFormatDateTimeSACH();
        public string NhanVienYeuCau { get; set; }

        // 0: chờ duyệt, 1: đã duyệt, 2: từ chối
        public int? TinhTrang => DaDuyet == true ? (int)TrangThaiYeuCauMuaDuTru.DaDuyet : (TuChoiDuyet == true ? (int)TrangThaiYeuCauMuaDuTru.TuChoi : (int)TrangThaiYeuCauMuaDuTru.ChoDuyet);

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

    public class KSNKTemplateLookupItem
    {
        public long KeyId { get; set; }
        public string DisplayName { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string DVT { get; set; }
        public string NhaSX { get; set; }
        public string NuocSX { get; set; }
        public string QuyCach { get; set; }
    }

    public class ThongTinChiTietKSNKTonKho
    {
        public long VatTuId { get; set; }
        public long KhoId { get; set; }
        public int LoaiVatTu { get; set; }
    }

    public class ThongTinDuTruMuaKSNK
    {
        public ThongTinDuTruMuaKSNK()
        {
            ThongTinChiTietTonKhoTongs = new List<ThongTinChiTietTonKho>();
            ThongTinChiTietTonHDTs = new List<ThongTinChiTietTonKho>();
        }
        public string DVT { get; set; }
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

    public class KSNKDuTruViewModelValidator
    {
        public long? VatTuId { get; set; }
        public bool? LaVatTuBHYT { get; set; }
    }
    public class PhieuMuaDuTruKSNK
    {
        public long? DuTruMuaVatTuId { get; set; }
        public long? DuTruMuaDuocPhamId { get; set; }

        public string HostingName { get; set; }
        public bool? Header { get; set; }
        public bool? TrangThai { get; set; }
    }
    public class YeuCauDuTruKSNKExcel
    {
        [Width(20)]
        public string SoPhieu { get; set; }
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

    public class PhieuMuaDuTruKSNKData
    {
        public string Header { get; set; }
        public string MaPhieuMuaDuTruVatTu { get; set; }
        public string VatTu { get; set; } // content
        public string KhoaPhong { get; set; }
        public string TenKho { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string NhanVienLap { get; set; }

    }

    public enum TrangThaiYeuCauMuaDuTru
    {
        [Description("Chờ duyệt")]
        ChoDuyet = 0,
        [Description("Đã duyệt")]
        DaDuyet = 1,
        [Description("Từ chối")]
        TuChoi = 2
    }
}
