namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class QuanLyTaiKhoanExportExcel
    {
        [Width(40)]
        public string HoTen { get; set; }
        [Width(25)]
        public string SoDienThoai { get; set; }
        [Width(50)]
        public string Email { get; set; }
        [Width(200)]
        public string DiaChi { get; set; }
        [Width(20)]
        public string IsActiveDisplay { get; set; }
    }
}
