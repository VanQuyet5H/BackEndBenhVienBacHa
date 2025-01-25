using Camino.Core.Domain.ValueObject.ExportExcelItemVo.Attributes;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.ExportExcelItemVo
{
    public class ListXacNhanBhytDaHoanThanhExportExcel
    {
        public string MaTn { get; set; }

        public string MaBn { get; set; }

        [Width(21)]
        public string HoTen { get; set; }

        public string NamSinh { get; set; }

        public string GioiTinh { get; set; }

        [Width(72)]
        public string DiaChi { get; set; }

        public string SoDienThoai { get; set; }

        [TextAlign(Constants.TextAlignAttribute.Right)]
        [Width(20)]
        public decimal? SoTienDaXacNhan { get; set; }
    }
}
