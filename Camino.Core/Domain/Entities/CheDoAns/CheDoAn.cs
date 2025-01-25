using Camino.Core.Domain.Entities.NhanViens;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.NhomChucDanhs;

namespace Camino.Core.Domain.Entities.CheDoAns
{
    public class CheDoAn : BaseEntity
    {
        public string Ten { get; set; }
        public string KyHieu { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }

        private ICollection<NoiTruPhieuDieuTri> _noiTruPhieuDieuTris;
        public virtual ICollection<NoiTruPhieuDieuTri> NoiTruPhieuDieuTris
        {
            get => _noiTruPhieuDieuTris ?? (_noiTruPhieuDieuTris = new List<NoiTruPhieuDieuTri>());
            protected set => _noiTruPhieuDieuTris = value;
        }
    }
}
