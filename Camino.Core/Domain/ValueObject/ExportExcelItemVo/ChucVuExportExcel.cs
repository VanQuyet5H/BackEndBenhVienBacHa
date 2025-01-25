namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ChucVuExportExcel
    {
        [Width(40)]
        public string Ten { get; set; }
        [Width(20)]
        public string TenVietTat { get; set; }
        [Width(20)]
        public string IsDisabled { get; set; }
    }
}
