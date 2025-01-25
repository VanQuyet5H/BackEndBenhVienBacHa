namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class VatTuBenhVienExportExcel
    {
        [Width(15)]
        public string Ma { get; set; }
        [Width(100)]
        public string Ten { get; set; }
        [Width(70)]
        public string TenNhomVatTu { get; set; }
        [Width(15)]
        public string TenDonViTinh { get; set; }
        [Width(16)]
        public string QuyCach { get; set; }
        [Width(16)]
        public string NhaSanXuat { get; set; }
        [Width(16)]
        public string NuocSanXuat { get; set; }
        [Width(20)]
        public string BaoHiemChiTra { get; set; }
        [Width(15)]
        public string HieuLuc { get; set; }
    }
}
