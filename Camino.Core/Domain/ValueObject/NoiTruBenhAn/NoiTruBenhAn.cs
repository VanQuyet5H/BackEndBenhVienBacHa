using Camino.Core.Domain.ValueObject.ExportExcelItemVo;
using Camino.Core.Domain.ValueObject.Grid;
using System;
using Camino.Core.Domain.ValueObject;
using Camino.Core.Helpers;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.ValueObject.NoiTruBenhAn
{
    public class DanhSachNoiTruBenhAnGridVo : GridItem
    {
        public string MaTiepNhan { get; set; }
        public string SoBenhAn { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
     
        //public string KhoaNhapVien { get; set; }

        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }

        public string NamSinhDisplay { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh?.GetDescription();

        public string Phong { get; set; }
        //public DateTime? ThoiGianTiepNhan { get; set; }
        //public string ThoiGianTiepNhanDisplay => ThoiGianTiepNhan?.ApplyFormatDateTimeSACH();

        public DateTime? ThoiGianNhapVien { get; set; }
        public string ThoiGianNhapVienDisplay => ThoiGianNhapVien?.ApplyFormatDateTimeSACH();

        public DateTime? ThoiGianRaVien { get; set; }
        public string ThoiGianRaVienDisplay => ThoiGianRaVien?.ApplyFormatDateTimeSACH();

        public string NoiChiDinh { get; set; }
        public string ChanDoan { get; set; }     
        public bool CapCuu { get; set; }
        public bool? CoBHYT { get; set; }
        public int? MucHuong { get; set; }
        public string DoiTuong => CoBHYT != true ? "Viện phí" : "BHYT (" + MucHuong + "%)";
        public int TrangThai { get; set; }
        public string TenTrangThai { get; set; }
        public long KhoaPhongId { get; set; }
        public bool? KetThucBenhAn { get; set; }
        public bool? DaQuyetToan { get; set; }

        public decimal SoTienConLai { get; set; }
    }

    public class NoiTruBenhAnModelSearch
    {
        public bool DangDieuTri { get; set; }
        public bool ChuyenVien { get; set; }
        public bool ChuyenKhoa { get; set; }
        public bool DaRaVien { get; set; }

        public long? KhoaPhongId { get; set; }
        public string SearchString { get; set; }

        public RangeDate TuNgayDenNgay { get; set; } //Nhập viện
        public RangeDate TuNgayDenNgayRaVien { get; set; } //Ra viện

        //public RangeDate RangeNhap { get; set; }
        //public RangeDate RangeDuyet { get; set; }
    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class DanhSachNoiTruExportExcel 
    {
        [Width(20)]
        public string Phong { get; set; }
        [Width(20)]
        public string MaTiepNhan { get; set; }
        [Width(20)]
        public string SoBenhAn { get; set; }
        [Width(20)]
        public string MaBenhNhan { get; set; }
        [Width(20)]
        public string HoTen { get; set; }
        [Width(20)]
        public string NamSinhDisplay { get; set; }
        [Width(20)]
        public string GioiTinhDisplay { get; set; }
        [Width(20)]
        public string KhoaNhapVien { get; set; }
        [Width(25)]
        public string ThoiGianNhapVienDisplay { get; set; }
        [Width(25)]
        public string ThoiGianRaVienDisplay { get; set; }
        [Width(25)]
        public string NoiChiDinh { get; set; }
        [Width(40)]
        public string ChanDoan { get; set; }
        [Width(20)]
        public string DoiTuong { get; set; }
        [Width(20)]
        public string CapCuu { get; set; }
        [Width(20)]
        public string TenTrangThai { get; set; }
        public bool? KetThucBenhAn { get; set; }
        public bool? DaQuyetToan { get; set; }
        public int TrangThai { get; set; }
    }
}
