using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.DanhSachDuocPhamCanLinhTrucTiep
{
    public class DanhSachDuocPhamCanLinhTrucTiepGridVo : GridItem
    {
        public long KhoLinhId { get; set; }
        public string KhoLinh { get; set; }
        public long PhongLinhVeId { get; set; }
        public string PhongLinhVe { get; set; }
    }
}
