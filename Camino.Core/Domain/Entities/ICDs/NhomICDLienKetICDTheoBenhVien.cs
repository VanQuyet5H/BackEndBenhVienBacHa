using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class NhomICDLienKetICDTheoBenhVien : BaseEntity
    {
        public long NhomICDTheoBenhVienId { get; set; }
        public long ICDId { get; set; }

        public virtual NhomICDTheoBenhVien NhomICDTheoBenhVien { get; set; }
        public virtual ICD ICD { get; set; }
    }
}
