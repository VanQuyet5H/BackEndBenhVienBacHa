using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoDoanhThuTheoBacSiQueryInfo : QueryInfo
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public long BacSiId { get; set; }
        public bool LayTatCa { get; set; }
    }
}
