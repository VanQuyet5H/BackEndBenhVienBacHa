using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoDoanhThuTheoNhomDichVuSearchQueryInfo 
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }     
    }

    public class BaoCaoThuPhiVienPhiQueryInfoQueryInfo : QueryInfo
    {
        public long? PhongBenhVienId { get; set; }
        public long? NhanVienId { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public bool LayTatCa { get; set; }
        public string TuNgayUTC { get; set; }
        public string DenNgayUTC { get; set; }
    }
}
