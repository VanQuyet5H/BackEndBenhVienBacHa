using Camino.Core.Domain.ValueObject.Grid;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.LyDoTiepNhan
{
    public class LyDoTiepNhanGridVo : GridItem
    {
        public string Ten { get; set; }
        public long? LyDoTiepNhanChaId { get; set; }
        public int CapNhom { get; set; }
        public string MoTa { get; set; }
        public virtual List<LyDoTiepNhanGridVo> LyDoTiepNhanChildList { get; set; }
    }

    public class LyDoTiepNhanDefaultDataGridVo : GridItem
    {
        public string Ten { get; set; }
    }
}
