namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class CheDoAnExportExcel
    {
        [Width(40)]
        public string Ten { get; set; }
        [Width(20)]
        public string KyHieu { get; set; }
        [Width(25)]
        public string MoTa { get; set; }
        [Width(20)]
        public string IsDisabled { get; set; }
    }
}
