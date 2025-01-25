namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class CongNoBenhNhanExportExcel
    {
        [Width(20)]
        public string MaBenhNhan { get; set; }
        [Width(30)]
        public string HoTen { get; set; }
        [Width(20)]
        public string GioiTinh { get; set; }
        [Width(20)]
        public int? NamSinh { get; set; }
        [Width(30)]
        public string SoDienThoai { get; set; }
        [Width(20)]
        public string DiaChi { get; set; }
    }
}
