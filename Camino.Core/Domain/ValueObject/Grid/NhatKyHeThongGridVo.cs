using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.Grid
{
    public class NhatKyHeThongGridVo:GridItem
    {
        public Enums.EnumNhatKyHeThong HoatDong { get; set; }
        public string TenHoatDong { get; set; }
        public string MaDoiTuong { get; set; }
        public long? IdDoiTuong { get; set; }
        public string NoiDung { get; set; }
        public long? NguoiTaoId { get; set; }
        public string NguoiTao { get; set; }
        public string NgayTaoFormat { get; set; }
        public DateTime? NgayTao { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }

    }
}
