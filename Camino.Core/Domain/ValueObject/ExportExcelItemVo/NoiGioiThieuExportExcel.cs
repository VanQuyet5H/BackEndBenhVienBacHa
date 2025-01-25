namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class NoiGioiThieuExportExcel
    {
        [Width(20)]
        public string Ten { get; set; }
        [Width(20)]
        public string DonVi { get; set; }
        [Width(20)]
        public string SoDienThoai { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
        [Width(20)]
        public string IsDisabled { get; set; }
        [Width(20)]
        public string HoTenNguoiQuanLy { get; set; }
    }
    public class CauHinhNoiGioiThieuVaHoaHongExportExcel
    {

        [Width(20)]
        public string Ten { get; set; }
        [Width(20)]
        public string DonVi { get; set; }
        [Width(20)]
        public string Sdt { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
    }
}
