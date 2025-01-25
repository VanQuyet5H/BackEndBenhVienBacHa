namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NhatKyHeThongExportExcel
    {
        [Width(20)]
        public string TenHoatDong { get; set; }
        [Width(40)]
        public string NoiDung { get; set; }
        [Width(30)]
        public string NgayTaoFormat { get; set; }
        [Width(20)]
        public string NguoiTao { get; set; }
    }
}
