using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoBienBanKiemKeKTQueryInfo : QueryInfo
    {
        public long KhoaPhongId { get; set; }
        public long KhoId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
