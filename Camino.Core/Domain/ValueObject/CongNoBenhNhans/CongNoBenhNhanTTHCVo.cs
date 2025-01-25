using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.CongNoBenhNhans
{
    public class CongNoBenhNhanTTHCVo : GridItem
    {
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh?.GetDescription();
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string DiaChi { get; set; }
        public bool ConNo { get; set; }
    }
}
