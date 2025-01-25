namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class KhoaPhongNhanVienExportExcel
    {
        [Group]
        public string TenKhoaPhong { get; set; }

        [Width(50)]
        public string TenNhanVien { get; set; }
    }
}
