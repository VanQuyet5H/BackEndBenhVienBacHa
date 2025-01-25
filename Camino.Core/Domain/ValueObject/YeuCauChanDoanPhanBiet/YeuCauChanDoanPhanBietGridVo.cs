using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauChanDoanPhanBiet
{
    public class YeuCauChanDoanPhanBietGridVo : GridItem
    {
        public long ICDId { get; set; }
        public string MaICd { get; set; }
        public string GhiChu { get; set; }
        public long YeuCauKhamBenhId { get; set; }

    }
}
