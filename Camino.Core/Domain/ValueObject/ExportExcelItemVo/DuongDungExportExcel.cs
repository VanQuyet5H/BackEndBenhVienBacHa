namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DuongDungExportExcel
    {
        [Width(15)]
        public string Ma { get; set; }
        [Width(40)]
        public string Ten { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
        [Width(20)]
        public string IsDisabled { get; set; }
    }
}
