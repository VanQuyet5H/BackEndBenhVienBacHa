namespace Camino.Core.Domain.Entities.BenhNhans
{
    public class BenhNhanDiUngThuoc : BaseEntity
    {
        public long BenhNhanId { get; set; }
        public string BieuHienDiUng { get; set; }
        public Enums.LoaiDiUng LoaiDiUng { get; set; }
        public string TenDiUng { get; set; }
        public Enums.EnumMucDoDiUng MucDo { get; set; }

        public virtual BenhNhan BenhNhan { get; set; }
    }
}
