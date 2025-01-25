using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.CauHinhs
{
    public class LoaiThuePhongPhauThuat : BaseEntity
    {
        public string Ten { get; set; }

        private ICollection<CauHinhThuePhong> _cauHinhThuePhongs;
        public virtual ICollection<CauHinhThuePhong> CauHinhThuePhongs
        {
            get => _cauHinhThuePhongs ?? (_cauHinhThuePhongs = new List<CauHinhThuePhong>());
            protected set => _cauHinhThuePhongs = value;
        }

        private ICollection<ThuePhong> _thuePhongs;
        public virtual ICollection<ThuePhong> ThuePhongs
        {
            get => _thuePhongs ?? (_thuePhongs = new List<ThuePhong>());
            protected set => _thuePhongs = value;
        }
    }
}
