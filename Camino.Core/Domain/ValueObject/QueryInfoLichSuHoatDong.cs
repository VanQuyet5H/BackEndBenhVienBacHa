using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.Grid
{
    public class QueryInfoLichSuHoatDong: QueryInfo
    {
        public Enums.EnumNhatKyHeThong HoatDongId { get; set; }
        public long NguoiTaoId { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
    }
}
