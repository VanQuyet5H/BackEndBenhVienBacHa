using System;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoTonKhoXetNghiemQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }
}