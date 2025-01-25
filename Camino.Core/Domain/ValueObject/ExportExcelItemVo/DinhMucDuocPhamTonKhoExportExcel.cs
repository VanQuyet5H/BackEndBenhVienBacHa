namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DinhMucDuocPhamTonKhoExportExcel
    {
        [Width(65)]
        public string TenDuocPham { get; set; }
        [Width(30)]
        public string TenKhoDuocPham { get; set; }
        [Width(20)]
        public int? TonToiThieu { get; set; }
        [Width(20)]
        public int? TonToiDa { get; set; }
        [Width(30)]
        public int? SoNgayTruocKhiHetHan { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
    }
}
