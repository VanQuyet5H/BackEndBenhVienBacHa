using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenhBoPhanTonThuong
{
    public class YeuCauKhamBenhBoPhanTonThuongGridVo : GridItem
    {
        public long YeuCauKhamBenhId { get; set; }
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }
    }
}
