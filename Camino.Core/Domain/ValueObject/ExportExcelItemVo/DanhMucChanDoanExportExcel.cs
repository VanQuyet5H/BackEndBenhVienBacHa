namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DanhMucChanDoanExportExcel
    {
        [Width(20)]
        public string TenTiengViet { get; set; }
        [Width(20)]
        public string TenTiengAnh { get; set; }
        [Width(20)]
        public string GhiChu { get; set; }
    }
}
