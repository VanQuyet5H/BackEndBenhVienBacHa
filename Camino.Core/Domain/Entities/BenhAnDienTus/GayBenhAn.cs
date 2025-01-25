using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.BenhAnDienTus
{
    public class GayBenhAn : BaseEntity
    {
        public string Ma { get; set; }
        public int ViTriGay { get; set; }
        public string Ten { get; set; }
        public bool? IsDisabled { get; set; }

        private ICollection<GayBenhAnPhieuHoSo> _gayBenhAnPhieuHoSos;
        public virtual ICollection<GayBenhAnPhieuHoSo> GayBenhAnPhieuHoSos
        {
            get => _gayBenhAnPhieuHoSos ?? (_gayBenhAnPhieuHoSos = new List<GayBenhAnPhieuHoSo>());
            protected set => _gayBenhAnPhieuHoSos = value;
        }
    }
}
