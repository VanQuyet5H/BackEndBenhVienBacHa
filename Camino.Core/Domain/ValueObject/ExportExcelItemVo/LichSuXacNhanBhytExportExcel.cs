using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class LichSuXacNhanBhytExportExcel
    {
        [Width(20)]
        public string IdLichSuXacNhan { get; set; }

        [Width(20)]
        public string MaTN { get; set; }

        [Width(20)]
        public string MaBN { get; set; }

        [Width(40)]
        public string HoTen { get; set; }

        [Width(20)]
        public string NamSinh { get; set; }

        [Width(20)]
        public string TenGioiTinh { get; set; }

        [Width(100)]
        public string DiaChi { get; set; }

        [Width(25)]
        public string SoDienThoai { get; set; }

        [Width(25)]
        [TextAlign(Constants.TextAlignAttribute.Right)]
        public decimal? SoTienDaXacNhan { get; set; }

        [Width(25)]
        public string ThoiDiemDuyetBaoHiem { get; set; }

        [Width(40)]
        public string NhanVienDuyetBaoHiem { get; set; }
    }
}
