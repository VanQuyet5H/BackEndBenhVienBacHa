using Camino.Core.Domain.ValueObject.ExportExcelItemVo;

namespace Camino.Core.Domain.ValueObject.HopDongThauDuocPham
{
    public class HopDongThauDuocPhamExportExcelChild
    {
        [TitleGridChild("Dược Phẩm")]
        public string DuocPham { get; set; }

        [TitleGridChild("Giá")]
        public string GiaDisplay { get; set; }

        [TitleGridChild("Số Lượng")]
        public string SoLuongDisplay { get; set; }

        [TitleGridChild("Số Lượng Đã Cung Cấp")]
        public string SoLuongCungCapDisplay { get; set; }
    }
}
