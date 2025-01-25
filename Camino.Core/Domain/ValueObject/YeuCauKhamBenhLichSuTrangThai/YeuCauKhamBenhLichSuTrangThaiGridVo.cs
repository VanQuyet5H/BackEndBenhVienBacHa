using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject
{
    public class YeuCauKhamBenhLichSuTrangThaiGridVo : GridItem
    {
        public long YeuCauKhamBenhId { get; set; }
        public Enums.EnumTrangThaiYeuCauKhamBenh TrangThaiYeuCauKhamBenh { get; set; }
    }
}
