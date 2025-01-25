using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class KetQuaCDHATDCNTimKiemVo
    {
        public string SearchString { get; set; }
        public KetQuaCDHATDCNTimKiemTrangThaiVo TrangThai { get; set; }
        public KetQuaCDHATDCNTimKiemTuNgayDenNgayVo TuNgayDenNgay { get; set; }
        public KetQuaCDHATDCNTimKiemTuNgayDenNgayVo ThucHienTuNgayDenNgay { get; set; }
    }

    public class KetQuaCDHATDCNTimKiemTrangThaiVo
    {
        public bool ChoKetQua { get; set; }
        public bool DaCoKetQua { get; set; }
    }

    public class KetQuaCDHATDCNTimKiemTuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }
}
