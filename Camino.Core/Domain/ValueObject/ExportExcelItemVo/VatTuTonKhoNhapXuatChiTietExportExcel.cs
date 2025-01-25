namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class VatTuTonKhoNhapXuatChiTietExportExcel
    {
        [Width(10)]
        public int? STT { get; set; }
        [Width(20)]
        public string NgayDisplay { get; set; }
        [Width(20)]
        public string MaChungTu { get; set; }
        [Width(20)]
        public double Nhap { get; set; }
        [Width(20)]
        public double Xuat { get; set; }
        [Width(20)]
        public double Ton { get; set; }
    }
}
