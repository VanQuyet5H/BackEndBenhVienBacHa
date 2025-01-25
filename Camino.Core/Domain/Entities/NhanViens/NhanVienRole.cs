using Camino.Core.Domain.Entities.Users;

namespace Camino.Core.Domain.Entities.NhanViens
{
    public class NhanVienRole : BaseEntity
    {
        public long NhanVienId { get; set; }
        public long RoleId { get; set; }
        public virtual Entities.NhanViens.NhanVien NhanVien { get; set; }
        public virtual Role Role { get; set; }
    }
}
