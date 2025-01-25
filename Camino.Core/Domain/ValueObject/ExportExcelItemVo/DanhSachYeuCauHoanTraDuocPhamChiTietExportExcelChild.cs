using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class DanhSachYeuCauHoanTraDuocPhamChiTietExportExcelChild
    {
        [TitleGridChild("HĐ Thầu")]
        public string HopDong { get; set; }

        [TitleGridChild("Dược Phẩm")]
        public string DuocPham { get; set; }

        [TitleGridChild("Loại")]
        public string Loai { get; set; }

        [TitleGridChild("Số Lô")]
        public string SoLo { get; set; }

        [TitleGridChild("Hạn Sử Dụng")]
        public string HsdText { get; set; }

        [TitleGridChild("Mã Vạch")]
        public string MaVach { get; set; }

        [TitleGridChild("SL Trả")]
        public double SoLuongTra { get; set; }

        [TitleGridChild("Đơn Giá Nhập")]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        [Width(16)]
        public decimal DonGiaNhap { get; set; }

        [TitleGridChild("VAT (%)")]
        [Width(10)]
        public int Vat { get; set; }

        [TitleGridChild("TL Tháp Giá")]
        [Width(14)]
        public int TiLeThapGia { get; set; }

        [TitleGridChild("Nhóm")]
        [Group]
        public string Nhom { get; set; }
    }
}
