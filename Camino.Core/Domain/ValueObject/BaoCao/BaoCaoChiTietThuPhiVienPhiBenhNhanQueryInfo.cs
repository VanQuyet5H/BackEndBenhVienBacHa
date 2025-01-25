using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoChiTietThuPhiVienPhiBenhNhanQueryInfo : QueryInfo
    {
        public long? PhongBenhVienId { get; set; }
        public long? NhanVienId { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public bool LayTatCa { get; set; }
    }
    public class BaoCaoChiTietThuPhiVienPhiBenhNhanConvertQueryInfo : QueryInfo
    {
        public string PhongBenhVienId { get; set; }
        public string NhanVienId { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public string LayTatCa { get; set; }
    }
}
