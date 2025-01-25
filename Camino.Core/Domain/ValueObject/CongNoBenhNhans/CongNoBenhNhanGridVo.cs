using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.CongNoBenhNhans
{
    public class CongNoBenhNhanGridVo : GridItem
    {
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh?.GetDescription();
        public string NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }

        //BVHD-3956
        public string NgayGhiNo { get; set; }
        public string MaTN { get; set; }
    }


    public class CongNoBenhNhanXuatExcel : GridItem
    {
        //public DateTime NgayGhiNo { get; set; }
        public string NgayGhiNoDisplay { get; set; }

        public string MaTN { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh?.GetDescription();
        public string NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }

        public decimal? SoTienPhaiThu { get; set; }
        public List<ThannhToanCongNo> ThanhToanCongNos { get; set; }
        public string LyDo { get; set; }
    }

    public class ThannhToanCongNo
    {
        public DateTime NgayThanhToan { get; set; }
        public string NgayThanhToanDisplay => NgayThanhToan.ApplyFormatDateTimeSACH();
        public decimal? SoTienPhaiThu { get; set; }
    }

    public class CongNoBenhNhanGridSearch
    {
        public string ToDate { get; set; }
        public string FromDate { get; set; }

        public DateTime? DTToDate { get; set; }
        public DateTime? DTFromDate { get; set; }

        public CongNoBenhNhan? CongNoId { get; set; }
        public string SearchString { get; set; }
    }

    public enum CongNoBenhNhan
    {
        [Description("Tất cả")]
        TatCa = 0,
        [Description("Ngoại trú")]
        NgoaiTru = 1,
        [Description("Nội trú")]
        NoiTru = 2,
        [Description("Nhà thuốc")]
        NhaThuoc = 3,
    }
}