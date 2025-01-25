using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class NhomICDTheoBenhVien : BaseEntity
    {
        public long ChuongICDId { get; set; }
        public string Stt { get; set; }
        public string Ma { get; set; }
        public string TenTiengViet { get; set; }
        public string TenTiengAnh { get; set; }
        public bool HieuLuc { get; set; }
        public string MoTa { get; set; }
        public virtual ChuongICD ChuongICD { get; set; }

        public ICollection<NhomICDLienKetICDTheoBenhVien> _nhomICDLienKetICDTheoBenhViens;
        public virtual ICollection<NhomICDLienKetICDTheoBenhVien> NhomICDLienKetICDTheoBenhViens
        {
            get => _nhomICDLienKetICDTheoBenhViens ?? (_nhomICDLienKetICDTheoBenhViens = new List<NhomICDLienKetICDTheoBenhVien>());
            protected set => _nhomICDLienKetICDTheoBenhViens = value;
        }
    }
}
