using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoTiepNhanBenhPhamQueryInfo : QueryInfo
    {
        public long DoanId { get; set; }
        //public long KhoaId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
