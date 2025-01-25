using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenhBoPhanKhac
{
    public class YeuCauKhamBenhBoPhanKhacGridVo : GridItem
    {
        public string Ten { get; set; }
        public string NoiDUng { get; set; }
        public long YeuCauKhamBenhId { get; set; }
    }
}
