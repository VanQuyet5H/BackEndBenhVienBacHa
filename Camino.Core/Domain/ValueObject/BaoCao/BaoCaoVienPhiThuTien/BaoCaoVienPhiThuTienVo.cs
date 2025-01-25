using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao.BaoCaoVienPhiThuTien
{
    public class BaoCaoVienPhiThuTienVo
    {
        public long NhanVienThuNganId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public string hosting { get; set; }
    }
}
