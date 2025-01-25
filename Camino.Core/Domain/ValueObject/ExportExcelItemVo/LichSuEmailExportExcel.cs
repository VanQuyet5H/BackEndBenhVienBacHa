namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class LichSuEmailExportExcel
    {
        [Width(40)]
        public string GoiDen { get; set; }
        [Width(25)]
        public string TieuDe { get; set; }
        [Width(80)]
        public string NoiDung { get; set; }
        [Width(20)]
        public string TapTinDinhKem { get; set; }
        [Width(20)]
        public string TenTrangThai { get; set; }
        [Width(30)]
        public string NgayGuiFormat { get; set; }
    }
}
