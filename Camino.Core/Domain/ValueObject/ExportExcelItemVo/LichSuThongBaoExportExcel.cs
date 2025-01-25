namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class LichSuThongBaoExportExcel
    {
        [Width(40)]
        public string GoiDen { get; set; }
        [Width(40)]
        public string NoiDung { get; set; }
        [Width(20)]
        public string TenTrangThai { get; set; }
        [Width(30)]
        public string NgayGui { get; set; }
    }
}
