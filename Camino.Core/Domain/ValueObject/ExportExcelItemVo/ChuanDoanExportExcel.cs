namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ChuanDoanExportExcel
    {
        [Width(20)]
        public string Ma { get; set; }

        [Width(136)]
        public string TenTiengViet { get; set; }

        [Width(136)]
        public string TenTiengAnh { get; set; }
    }
}
