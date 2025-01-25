using System;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien
{
    public class DichVuGiuongBenhVienGridVo : GridItem
    {
        public decimal Gia { get; set; }

        public string GiaDisplay { get; set; }

        public string TuNgayDisplay { get; set; }

        public string DenNgayDisplay { get; set; }

        public string MoTa { get; set; }

        public DateTime? TuNgay { get; set; }

        public bool? HieuLuc { get; set; }
    }
}
