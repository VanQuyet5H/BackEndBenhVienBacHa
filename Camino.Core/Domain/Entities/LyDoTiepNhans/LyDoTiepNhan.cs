using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.LyDoTiepNhans
{
    public class LyDoTiepNhan : BaseEntity
    {
        public string Ten { get; set; }
        public long? LyDoTiepNhanChaId { get; set; }
        public int CapNhom { get; set; }
        public string MoTa { get; set; }

        public virtual LyDoTiepNhan LyDoTiepNhanCha { get; set; }

        private ICollection<LyDoTiepNhan> _lyDoTiepNhans;
        public virtual ICollection<LyDoTiepNhan> LyDoTiepNhans
        {
            get => _lyDoTiepNhans ?? (_lyDoTiepNhans = new List<LyDoTiepNhan>());
            protected set => _lyDoTiepNhans = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }
    }
}
