using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.LichSuKhamChuaBenhs
{
    public class LichSuKhamChuaBenhTimKiemVo
    {
        public string SearchString { get; set; }
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public LichSuKhamChuaBenhTimKiemTuNgayDenNgayVo TuNgayDenNgay { get; set; }
    }

    public class LichSuKhamChuaBenhTimKiemTuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }
    public class InDichVuKyThuatVo
    {
        public string Hosting { get; set; }
        public long YeuCauTiepNhanId { get; set; }
    }
}
