namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class PhanQuyenNguoiDungExportExcel
    {
        [Width(22)]
        public string Ten { get; set; }

        public string LoaiNguoiDung { get; set; }

        public string Quyen { get; set; }
    }
}
