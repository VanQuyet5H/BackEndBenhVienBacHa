using Camino.Core.Domain.Entities;
using Camino.Core.Domain.Entities.NhanVienChucVus;
using Camino.Core.Domain.Entities.NhanViens;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.ChucVus
{
    public class ChucVu : BaseEntity
    {
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }

        private ICollection<NhanVienChucVu> _nhanVienChucVus;
        public virtual ICollection<NhanVienChucVu> NhanVienChucVus
        {
            get => _nhanVienChucVus ?? (_nhanVienChucVus = new List<NhanVienChucVu>());
            protected set => _nhanVienChucVus = value;
        }
    }
}
