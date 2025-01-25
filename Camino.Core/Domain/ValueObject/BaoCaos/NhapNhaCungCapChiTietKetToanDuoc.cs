using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.ComponentModel;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class NhapNhaCungCapChiTietKeToanDuocQueryInfoVo : QueryInfo
    {
        public long KhoNhapId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string TimKiem { get; set; }
        public LoaiNgayTimKiem LoaiNgayTimKiem { get; set; }
    }

    public enum LoaiNgayTimKiem
    {
        [Description("Ngày nhập")]
        NgayNhap = 1,
        [Description("Ngày duyệt nhập")]
        NgayDuyetNhap = 2,
        [Description("Ngày hóa đơn")]
        NgayHoaDon = 3,
    }

    public class NhapNhaCungCapChiTietKeToanDuoc : GridItem
    {
        public string NhomNhaCungCap { get; set; }
        public string KhoNhap { get; set; }

        public DateTime? NgayChungTu { get; set; }
        public string NgayChungTuDisplay => NgayChungTu?.ApplyFormatDateTimeSACH();

        public string SoChungTu { get; set; }

        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonDisplay => NgayHoaDon?.ApplyFormatDateTimeSACH();

        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public string HoatChat { get; set; }
        public string QuyCachDongGoi { get; set; }
        public string SoLo { get; set; }

        public DateTime? HanDung { get; set; }
        public string HanDungDisplay => HanDung?.ApplyFormatDateTimeSACH();

        public string DVT { get; set; }
        public double SoLuongNhap { get; set; }
        public decimal DonGiaNhap { get; set; }

        public decimal ThanhTien => (decimal)SoLuongNhap * DonGiaNhap;
        public decimal VAT { get; set; }
        public decimal Tong => ThanhTien + VAT;
        public decimal DonGiaCoVat { get; set; }

        public string GhiChu { get; set; }
    }
}
