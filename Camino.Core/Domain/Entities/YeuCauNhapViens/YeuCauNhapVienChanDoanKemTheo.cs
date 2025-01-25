using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;

namespace Camino.Core.Domain.Entities.YeuCauNhapViens
{
    public class YeuCauNhapVienChanDoanKemTheo : BaseEntity
    {
        public long YeuCauNhapVienId { get; set; }
        public long ICDId { get; set; }
        public string GhiChu { get; set; }

        public virtual YeuCauNhapVien YeuCauNhapVien { get; set; }
        public virtual ICD ICD { get; set; }
    }
}
