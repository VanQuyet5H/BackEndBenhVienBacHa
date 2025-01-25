using Camino.Core.Domain.Entities.Users;

namespace Camino.Core.Domain.Entities.HeThong
{
    public class NhatKyHeThong:BaseEntity
    {
        public Enums.EnumNhatKyHeThong HoatDong { get; set; }
        public string MaDoiTuong { get; set; }
        public long? IdDoiTuong { get; set; }
        public string NoiDung { get; set; }
        public virtual User UserDetails { get; set; }
    }
}
