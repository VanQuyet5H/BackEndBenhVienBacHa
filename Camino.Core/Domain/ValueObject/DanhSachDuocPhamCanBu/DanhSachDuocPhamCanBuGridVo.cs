using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DanhSachDuocPhamCanBu
{
    public class DanhSachDuocPhamCanBuGridVo : GridItem
    {
        public long KhoLinhId { get; set; }
        public string KhoLinh { get; set; }
        public long KhoBuId { get; set; }
        public string KhoBu { get; set; }
    }
}
