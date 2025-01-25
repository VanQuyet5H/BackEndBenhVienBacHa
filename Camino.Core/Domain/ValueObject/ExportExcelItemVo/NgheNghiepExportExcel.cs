namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NgheNghiepExportExcel
    {
        [Width(20)]
        public string TenVietTat { get; set; }
        [Width(30)]
        public string Ten { get; set; }
        [Width(30)]
        public string MoTa { get; set; }
        [Width(20)]
        public string IsDisabled { get; set; }
    }
}
