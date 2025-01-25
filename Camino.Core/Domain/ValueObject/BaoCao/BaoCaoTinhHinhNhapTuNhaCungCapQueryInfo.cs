using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTinhHinhNhapTuNhaCungCapQueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
        public bool TheoThoiGianNhap { get; set; }

        public int Skip { get; set; }
        public int Take { get; set; }
    }
}
