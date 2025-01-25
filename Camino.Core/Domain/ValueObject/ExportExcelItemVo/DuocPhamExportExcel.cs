namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DuocPhamExportExcel
    {
        [Width(50)]
        public string Ten { get; set; }
        [Width(30)]
        public string SoDangKy { get; set; }
        [Width(30)]
        public string MaHoatChat { get; set; }
        [Width(30)]
        public string HoatChat { get; set; }
        [Width(30)]
        public string QuyCach { get; set; }
        [Width(40)]
        public string TenDonViTinh { get; set; }
        [Width(35)]
        public string TenDuongDung { get; set; }
        [Width(30)]
        public string TenLoaiThuocHoacHoatChat { get; set; }
        [Width(40)]
        public int? HeSoDinhMucDonViTinh { get; set; }
    }
}
