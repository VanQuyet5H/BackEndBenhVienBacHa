using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.YeuCauTiepNhans
{
    public class LoaiHoSoYeuCauTiepNhan : BaseEntity
    {
        public string Ten { get; set; }
        public bool IsDefault { get; set; }

        private ICollection<HoSoYeuCauTiepNhan> _hoSoYeuCauTiepNhans;
        public virtual ICollection<HoSoYeuCauTiepNhan> HoSoYeuCauTiepNhans
        {
            get => _hoSoYeuCauTiepNhans ?? (_hoSoYeuCauTiepNhans = new List<HoSoYeuCauTiepNhan>());
            protected set => _hoSoYeuCauTiepNhans = value;
        }
    }
}