using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCao.BaoCaoBienBanKiemKeDPVT
{
    public class BaoCaoBienBanKiemKeDPVTQueryInfo:QueryInfo
    {
        public long KhoId { get; set; }
        public DateTime GioThongKe { get; set; }
    }
}
