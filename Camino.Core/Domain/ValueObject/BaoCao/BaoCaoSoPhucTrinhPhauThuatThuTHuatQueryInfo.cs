using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoSoPhucTrinhPhauThuatThuTHuatQueryInfo : QueryInfo
    {
        public BaoCaoSoPhucTrinhPhauThuatThuTHuatQueryInfo()
        {
            KhoaPhongIds = new List<string>();
            NoiThucHienIds = new List<string>();
        }

        public long? KhoaId { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public DateTime TuNgayFormat { get; set; }
        public DateTime DenNgayFormat { get; set; }

        //BVHD-3636
        public List<string> KhoaPhongIds { get; set; }
        public List<string> NoiThucHienIds { get; set; }
        public bool? LaPhauThuat { get; set; }
        public bool? LaThuThuat { get; set; }
    }
}