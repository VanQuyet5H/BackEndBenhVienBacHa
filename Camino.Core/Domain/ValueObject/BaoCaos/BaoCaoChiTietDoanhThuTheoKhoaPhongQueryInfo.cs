using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoChiTietDoanhThuTheoKhoaPhongQueryInfo : QueryInfo
    {
        public long? KhoaPhongId { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public DateTime? KySoSanhTuNgay { get; set; }
        public DateTime? KySoSanhDenNgay { get; set; }
        public bool LayTatCa { get; set; }
    }
}
