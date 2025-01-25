namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class KhoDuocPhamViTriExportExcel
    {
        [Width(105)]
        public string Ten { get; set; }
        [Width(35)]
        public string MoTa { get; set; }
        [Width(20)]
        public string IsDisabled { get; set; }
    }
}
