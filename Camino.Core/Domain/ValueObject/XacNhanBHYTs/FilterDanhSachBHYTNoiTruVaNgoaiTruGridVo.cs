using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.XacNhanBHYTs
{
    public class FilterDanhSachBHYTNoiTruVaNgoaiTruGridVo : QueryInfo
    {
        public bool? NgoaiTru { get; set; }
        public bool? NoiTru { get; set; }
        //public string SearchString { get; set; }

        public DateTime? ThoiDiemTiepNhanTu { get; set; }
        public DateTime? ThoiDiemTiepNhanDen { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }

    public class DanhSachXacNhanHoanThanhNoiTruVaNgoaiTru : GridItem
    {
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public string ThoiDiemTiepNhanDisplay => ThoiDiemTiepNhan != null ? ThoiDiemTiepNhan.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        //public string MaXacNhan { get; set; }
        public string MaTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string NamSinh { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }

        public decimal SoTienDaXacNhan { get; set; }
        public DateTime? ThoiDiemDuyet { get; set; }
        public string ThoiDiemDuyetDisplay => ThoiDiemDuyet != null ? ThoiDiemDuyet.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        public string LoaiDieuTri { get; set; }
        public string NguoiDuyet { get; set; }
    }

    public class DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataDuyet
    {
        public long? YeuCauTiepNhanNgoaiTruId { get; set; }
        public long? YeuCauTiepNhanNoiTruId { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }        
        public string MaTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime ThoiDiemDuyet { get; set; }
        public long NhanVienDuyetId { get; set; }
    }
    public class DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataTiepNhan
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemTiepNhan { get; set; }
        public string MaTiepNhan { get; set; }
        public Enums.EnumLoaiYeuCauTiepNhan LoaiYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
    }
    public class DanhSachXacNhanHoanThanhNoiTruVaNgoaiTruDataSoTienXacNhan
    {
        public long YeuCauTiepNhanId { get; set; }
        public double Soluong { get; set; }
        public int TiLeBaoHiemThanhToan { get; set; }
        public int MucHuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal DonGiaBHYTThanhToan => DonGia * TiLeBaoHiemThanhToan / 100 * MucHuong / 100;
        public decimal SoTienDaXacNhan => (decimal)(Soluong * (double)DonGiaBHYTThanhToan);
    }
}
