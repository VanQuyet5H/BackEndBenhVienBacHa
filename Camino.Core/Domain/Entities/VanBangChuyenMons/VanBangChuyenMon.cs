using Camino.Core.Domain.Entities.NhanViens;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.VanBangChuyenMons
{
    public class VanBangChuyenMon : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenVietTat { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }

        private ICollection<NhanVien> _nhanViens;
        public virtual ICollection<NhanVien> NhanViens
        {
            get => _nhanViens ?? (_nhanViens = new List<NhanVien>());
            protected set => _nhanViens = value;
        }
    }
}
