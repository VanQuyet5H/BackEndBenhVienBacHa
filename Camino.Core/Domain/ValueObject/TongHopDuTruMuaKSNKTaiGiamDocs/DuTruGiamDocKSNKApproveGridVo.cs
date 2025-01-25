using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.TongHopDuTruMuaKSNKTaiGiamDocs
{
    public class DuTruGiamDocKSNKApproveGridVo : GridItem
    {
        public string LyDo { get; set; }
        public List<ChiTietKSNKGridVo> ChiTietKSNKList { get; set; }
    }

    public class ChiTietKSNKGridVo : GridItem
    {
        public int? SoLuongDuyet { get; set; }
    }
}
