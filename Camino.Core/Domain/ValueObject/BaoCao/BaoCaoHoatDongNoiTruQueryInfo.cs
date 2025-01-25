using System;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class BaoCaoHoatDongNoiTruQueryInfo : QueryInfo
    {
        public long NoiDieuTriId { get; set; }//enum
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}