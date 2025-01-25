using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.TrieuChungDanhMucChuanDoans
{
    public class TrieuChungDanhMucChuanDoanGridVo : GridItem
    {
        public long TrieuChungId { get; set; }
        public long DanhMucChuanDoanId { get; set; }
    }
}
