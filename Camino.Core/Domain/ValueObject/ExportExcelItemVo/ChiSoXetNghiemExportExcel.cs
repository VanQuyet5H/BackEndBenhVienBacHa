namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ChiSoXetNghiemExportExcel
    {
        [Width(20)]
        public string Ma { get; set; }
        [Width(20)]
        public string Ten { get; set; }
        [Width(20)]
        public string TenTiengAnh { get; set; }
        [Width(25)]
        public string ChiSoBinhThuongNam { get; set; }
        [Width(25)]
        public string ChiSoBinhThuongNu { get; set; }
        [Width(30)]
        public string TenLoaiXetNghiem { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
        [Width(20)]
        public string HieuLuc { get; set; }
    }
}
