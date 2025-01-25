using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DanhSachVatTuCanBu
{
    public class DanhSachVatTuCanBuGridVo : GridItem
    {
        public long KhoLinhId { get; set; }
        public string KhoLinh { get; set; }
        public long KhoBuId { get; set; }
        public string KhoBu { get; set; }
    }
}
