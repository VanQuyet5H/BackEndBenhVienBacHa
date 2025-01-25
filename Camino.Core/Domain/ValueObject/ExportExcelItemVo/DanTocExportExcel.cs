namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DanTocExportExcel
    {
        [Width(15)]
        public string Ma { get; set; }
        [Width(20)]
        public string Ten { get; set; }
        [Width(20)]
        public string TenQuocGia { get; set; }
        [Width(20)]
        public string IsDisabled { get; set; }
    }
}
