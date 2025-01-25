using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.GoiDvMarketings
{
    public class ChuongTrinhGoiDvMarketingGridVo : GridItem
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenDv { get; set; }

        public decimal GiaTruocChietKhau { get; set; }

        public double TiLeChietKhau => GiaTruocChietKhau != 0 ? (double)((GiaTruocChietKhau - GiaSauChietKhau) / GiaTruocChietKhau * 100) : 0;

        public decimal GiaSauChietKhau { get; set; }

        public DateTime TuNgay { get; set; }

        public string TuNgayDisplay => TuNgay.ApplyFormatDate();

        public DateTime? DenNgay { get; set; }

        public string DenNgayDisplay => DenNgay != null ? DenNgay.GetValueOrDefault().ApplyFormatDate() : string.Empty;

        public bool? TamNgung { get; set; }
    }

    public class ChuongTrinhGoiDvMarketingSearch
    {
        public string SearchString { get; set; }

        public RangeDate RangeFromDate { get; set; }

        public RangeDate RangeToDate { get; set; }
    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }
    }

    public class LoaiGoiDichVuGridVo : GridItem
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool IsDefault { get; set; }
    }
}
