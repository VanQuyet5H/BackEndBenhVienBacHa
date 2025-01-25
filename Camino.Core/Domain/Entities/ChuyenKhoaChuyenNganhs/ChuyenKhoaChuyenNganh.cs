using Camino.Core.Domain.Entities.DichVuKyThuats;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.ChuyenKhoaChuyenNganh
{
    public class ChuyenKhoaChuyenNganh : BaseEntity
    {
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }

        private ICollection<DichVuKyThuatBenhVien> _dichVuKyThuatBenhViens;
        public virtual ICollection<DichVuKyThuatBenhVien> DichVuKyThuatBenhViens
        {
            get => _dichVuKyThuatBenhViens ?? (_dichVuKyThuatBenhViens = new List<DichVuKyThuatBenhVien>());
            protected set => _dichVuKyThuatBenhViens = value;
        }
    }
}
