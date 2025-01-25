using System.Collections.Generic;
using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.Users
{
    public class Role : BaseEntity
    {
        private ICollection<RoleFunction> _roleFunctions;

        public string Name { get; set; }
        public Enums.UserType UserType { get; set; }
        public bool IsDefault { get; set; }

        public virtual ICollection<RoleFunction> RoleFunctions
        {
            get => _roleFunctions ?? (_roleFunctions = new List<RoleFunction>());
            protected set => _roleFunctions = value;
        }

        private ICollection<NhanVienRole> _nhanVienRoles;
        public virtual ICollection<NhanVienRole> NhanVienRoles
        {
            get => _nhanVienRoles ?? (_nhanVienRoles = new List<NhanVienRole>());
            protected set => _nhanVienRoles = value;
        }
    }
}
