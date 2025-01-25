namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class HopDongThauVatTuExportExcelChild
    {
        [TitleGridChild("Vật Tư")]
        public string VatTu { get; set; }

        [TitleGridChild("Giá")]
        public string Gia { get; set; }

        [TitleGridChild("Số Lượng")]
        public string SoLuongDisplay { get; set; }

        [TitleGridChild("Số Lượng Đã Cung Cấp")]
        public string SoLuongCungCapDisplay { get; set; }
    }
}
