using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class NhomICD : BaseICDEntity
    {
        public long ChuongICDId { get; set; }
        public long? NhomICDChaId { get; set; }
        public int CapNhomICD { get; set; }

        public virtual ChuongICD ChuongICD { get; set; }

        public ICollection<LoaiICD> _loaiICDs;
        public virtual ICollection<LoaiICD> LoaiICDs
        {
            get => _loaiICDs ?? (_loaiICDs = new List<LoaiICD>());
            protected set => _loaiICDs = value;
        }
    }
}
