using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.BenhVien.LoaiBenhViens
{
    public class LoaiBenhVien : BaseEntity
    {
        public string Ten { get; set; }

        public string MoTa { get; set; }

        private ICollection<BenhVien> _benhViens;
        public virtual ICollection<BenhVien> BenhViens
        {
            get => _benhViens ?? (_benhViens = new List<BenhVien>());
            protected set => _benhViens = value;
        }
    }
}
