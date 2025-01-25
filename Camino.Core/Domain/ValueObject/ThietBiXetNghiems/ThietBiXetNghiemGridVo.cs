using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.ThietBiXetNghiems
{
    public class ThietBiXetNghiemGridVo : GridItem
    {
        public string NhomXetNghiemDisplay { get; set; }

        public string NhomThietBiDisplay { get; set; }

        public string Ma { get; set; }

        public string Ten { get; set; }

        public string Ncc { get; set; }

        public Enums.EnumConnectionStatus TinhTrang { get; set; }

        public string TinhTrangDisplay => TinhTrang == Enums.EnumConnectionStatus.Open
            ? "<span class='green-txt'>Mở</span>"
            : "<span class='red-txt'>Đóng</span>";

        public bool HieuLuc { get; set; }
    }
}
