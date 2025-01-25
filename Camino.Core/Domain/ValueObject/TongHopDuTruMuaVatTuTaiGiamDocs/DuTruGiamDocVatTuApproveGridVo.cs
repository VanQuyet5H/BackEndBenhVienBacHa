using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.TongHopDuTruMuaVatTuTaiGiamDocs
{
    public class DuTruGiamDocVatTuApproveGridVo : GridItem
    {
        public string LyDo { get; set; }
        public List<ChiTietVatTuGridVo> ChiTietVatTuList { get; set; }
    }

    public class ChiTietVatTuGridVo : GridItem
    {
        public int? SoLuongDuyet { get; set; }
    }
}
