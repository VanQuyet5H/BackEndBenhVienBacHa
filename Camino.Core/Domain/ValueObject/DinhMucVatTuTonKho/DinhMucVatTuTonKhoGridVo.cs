using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DinhMucVatTuTonKho
{
    public class DinhMucVatTuTonKhoGridVo : GridItem
    {
        public string Kho { get; set; }

        public string TenVt { get; set; }

        public int? TonToiThieu { get; set; }

        public int? TonToiDa { get; set; }

        public int? SoNgayTruocKhiHetHan { get; set; }

        public string MoTa { get; set; }
    }
}
