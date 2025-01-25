using Camino.Core.Domain.ValueObject.Grid;
using System;

namespace Camino.Core.Domain.ValueObject.DichVuGiuong
{
    public class DichVuGiuongThongTinGiaGridVo : GridItem
    {
        public decimal? Gia { get; set; }
        public string GiaDisplay { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string TuNgayDisplay { get; set; }
        public string DenNgayDisplay { get; set; }
        public string MoTa { get; set; }
        public bool? HieuLuc { get; set; }
        public string HieuLucDisplay { get; set; }
    }

}
