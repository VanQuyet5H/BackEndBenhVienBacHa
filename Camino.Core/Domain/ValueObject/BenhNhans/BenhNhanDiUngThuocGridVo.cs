using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BenhNhans
{
    public class BenhNhanDiUngThuocGridVo : GridItem
    {
        public long BenhNhanId { get; set; }

        public string MaHoatChat { get; set; }

        public string HoatChat { get; set; }

    }

    public class BenhNhanDiUngThuocKhamBenhGridVo : GridItem
    {
        public string TenThuoc { get; set; }

        public string BieuHienDiUng { get; set; }
        public long BenhNhanId { get; set; }
        public Enums.LoaiDiUng LoaiDiUng { get; set; }
        public string TenLoaiDiUng { get; set; }

        public string TenDiUng { get; set; }
        public Enums.EnumMucDoDiUng MucDo { get; set; }
        public string TenMucDo { get; set; }
    }
}
