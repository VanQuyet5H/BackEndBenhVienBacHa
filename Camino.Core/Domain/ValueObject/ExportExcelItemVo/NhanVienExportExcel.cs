namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NhanVienExportExcel
    {
        [Width(40)]
        public string HoTen { get; set; }
        [Width(25)]
        public string SoChungMinhThu { get; set; }
        [Width(25)]
        public string SoDienThoai { get; set; }
        [Width(50)]
        public string Email { get; set; }
        [Width(200)]
        public string DiaChi { get; set; }
    }
}
