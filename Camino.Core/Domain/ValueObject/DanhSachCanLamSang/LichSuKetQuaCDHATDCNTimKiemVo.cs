using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class LichSuKetQuaCDHATDCNTimKiemVo
    {
        public string SearchString { get; set; }
        public LichSuKetQuaCDHATDCNTimKiemTuNgayDenNgayVo TuNgayDenNgay { get; set; }
    }

    public class LichSuKetQuaCDHATDCNTimKiemTuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }
}
