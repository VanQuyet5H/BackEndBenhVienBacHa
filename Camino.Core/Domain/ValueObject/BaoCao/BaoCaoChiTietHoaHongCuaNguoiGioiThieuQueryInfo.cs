using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoChiTietHoaHongCuaNguoiGioiThieuQueryInfo: QueryInfo
    {
        public long? HinhThucDenId { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }
    }
}
