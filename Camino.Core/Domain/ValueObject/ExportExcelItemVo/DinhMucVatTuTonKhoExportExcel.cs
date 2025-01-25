using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DinhMucVatTuTonKhoExportExcel : GridItem
    {
        public string Kho { get; set; }

        [Width(52)]
        public string TenVt { get; set; }

        public int? TonToiThieu { get; set; }

        public int? TonToiDa { get; set; }

        [Width(28)]
        public int? SoNgayTruocKhiHetHan { get; set; }

        [Width(110)]
        public string MoTa { get; set; }
    }
}
