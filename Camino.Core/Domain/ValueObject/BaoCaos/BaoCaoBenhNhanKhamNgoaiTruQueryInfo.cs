using System;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoBenhNhanKhamNgoaiTruQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long? KhoaId { get; set; }
        public long? PhongId { get; set; }
    }
}