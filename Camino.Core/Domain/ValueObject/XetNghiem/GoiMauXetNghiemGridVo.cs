using Camino.Core.Domain.Entities.XetNghiems;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.XetNghiem
{
    public class GoiMauDanhSachXetNghiemGridVo : GridItem
    {
        public GoiMauDanhSachXetNghiemGridVo()
        {
            GoiMauDanhSachNhomXetNghiems = new List<GoiMauDanhSachNhomXetNghiemGridVo>();
        }

        public string SoPhieu { get; set; }
        public long NguoiGoiMauId { get; set; }
        public string NguoiGoiMauDisplay { get; set; }
        public DateTime NgayGoiMau { get; set; }
        public string NgayGoiMauDisplay => NgayGoiMau.ApplyFormatDateTimeSACH();
        //public string SoLuongMau { get; set; }
        public bool? TinhTrang { get; set; }
        public string TinhTrangDisplay => TinhTrang == true ? "Đã nhận mẫu" : "Chờ nhận mẫu";
        public long NoiTiepNhanId { get; set; }
        public string NoiTiepNhan { get; set; }
        public long? NguoiNhanMauId { get; set; }
        public string NguoiNhanMauDisplay { get; set; }
        public DateTime? NgayNhanMau { get; set; }
        public string NgayNhanMauDisplay => NgayNhanMau == null ? "" : NgayNhanMau.Value.ApplyFormatDateTimeSACH();

        public List<GoiMauDanhSachNhomXetNghiemGridVo> GoiMauDanhSachNhomXetNghiems { get; set; }
    }

    public class GoiMauDanhSachNhomXetNghiemGridVo : GridItem
    {
        public GoiMauDanhSachNhomXetNghiemGridVo()
        {
            //LoaiMauXetNghiems = new List<EnumLoaiMauXetNghiem>();
            GoiMauDanhSachDichVuXetNghiems = new List<GoiMauDanhSachDichVuXetNghiemGridVo>();
        }


        public long PhieuGoiMauXetNghiemId { get; set; }
        public string SoPhieu { get; set; }
        public long PhienXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public string NhomDichVuBenhVienDisplay { get; set; }
        public string Barcode { get; set; }
        public string BarCodeNumber { get; set; }
        public string MaTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh == null ? "" : GioiTinh.GetDescription();
        public LoaiMauNhanMauXetNghiemVo LoaiMauXetNghiem { get; set; }
        public List<LoaiMauNhanMauXetNghiemVo> LoaiMauXetNghiems { get; set; }

        public List<GoiMauDanhSachDichVuXetNghiemGridVo> GoiMauDanhSachDichVuXetNghiems { get; set; }

        public PhieuGoiMauXetNghiem PhieuGoiMauXetNghiem { get; set; }
    }

    public class GoiMauDanhSachDichVuXetNghiemGridVo : GridItem
    {
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public DateTime ThoiGianChiDinh { get; set; }
        public string ThoiGianChiDinhDisplay => ThoiGianChiDinh.ApplyFormatDateTimeSACH();
        public long NguoiChiDinhId { get; set; }
        public string NguoiChiDinhDisplay { get; set; }
        public string BenhPham { get; set; }
        public EnumLoaiMauXetNghiem? LoaiMau { get; set; }
        public string LoaiMauDisplay => LoaiMau == null ? "" : LoaiMau.GetDescription();
        public int LanThucHien { get; set; }

        public long YeuCauDichVuKyThuatId { get; set; }
    }

    public class GoiMauXetNghiemSearch
    {
        public bool ChoNhanMau { get; set; }
        public bool DaNhanMau { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeNgayGoiMau { get; set; }
        //public DateTime? TuNgay { get; set; }
        //public DateTime? DenNgay { get; set; }
        public long? PhieuGoiMauXetNghiemId { get; set; }
    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class DichVuXetNghiemSearch
    {
        public long PhienXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
    }

    public class DanhSachKhongTiepNhanMauGridVo : GridItem
    {
        public EnumLoaiMauXetNghiem LoaiMauXetNghiem { get; set; }
        public string LoaiMauXetNghiemDisplay => LoaiMauXetNghiem.GetDescription();
        //public bool? DatChatLuong { get; set; }
        public bool? KhongDatChatLuong { get; set; }
        public string LyDoKhongDat { get; set; }
    }

    public class KhongTiepNhanMauSearch
    {
        public long PhieuGoiMauXetNghiemId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        public long PhienXetNghiemId { get; set; }
    }
}