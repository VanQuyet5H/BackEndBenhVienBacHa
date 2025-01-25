using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruThamKhamChanDoanKemTheo : BaseEntity
    {
        public long NoiTruPhieuDieuTriId { get; set; }
        public long ICDId { get; set; }
        public string GhiChu { get; set; }

        public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }
        public virtual ICD ICD { get; set; }
    }
}
