using System.Collections.Generic;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.TongHopDuTruMuaThuocTaiGiamDocs
{
    public class DuTruGiamDocApproveGridVo : GridItem
    {
        public string LyDo { get; set; }
        public List<ChiTietDuocPhamGridVo> ChiTietDuocPhamList { get; set; }
    }

    public class ChiTietDuocPhamGridVo : GridItem
    {
        public int? SoLuongDuyet { get; set; }
    }
}
