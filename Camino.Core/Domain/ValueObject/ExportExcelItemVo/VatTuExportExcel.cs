namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class VatTuExportExcel
    {
        [Width(20)]
        public string Ma { get; set; }
        [Width(95)]
        public string Ten { get; set; }
        [Width(70)]
        public string TenNhomVatTu { get; set; }
        [Width(20)]
        public string TenDonViTinh { get; set; }
        [Width(30)]
        public int? TyLeBaoHiemThanhToan { get; set; }
        [Width(20)]
        public string QuyCach { get; set; }
        [Width(20)]
        public string NhaSanXuat { get; set; }
        [Width(20)]
        public string NuocSanXuat { get; set; }
        [Width(20)]
        public string MoTa { get; set; }
        [Width(20)]
        public string HeSoDinhMucDonViTinh { get; set; }        
        [Width(20)]
        public string IsDisabled { get; set; }
    }
}
