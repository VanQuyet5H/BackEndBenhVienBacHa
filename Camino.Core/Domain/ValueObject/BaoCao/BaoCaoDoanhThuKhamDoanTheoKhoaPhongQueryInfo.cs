using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoDoanhThuKhamDoanTheoKhoaPhongQueryInfo: QueryInfo
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }
        public string Hosting { get; set; }
    }
}
