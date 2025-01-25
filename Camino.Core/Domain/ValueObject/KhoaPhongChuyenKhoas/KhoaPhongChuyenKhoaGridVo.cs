using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.KhoaPhongChuyenKhoas
{
    public class KhoaPhongChuyenKhoaGridVo : GridItem
    {
        public long KhoaPhongId { get; set; }
        public long KhoaId { get; set; }
    }
}
