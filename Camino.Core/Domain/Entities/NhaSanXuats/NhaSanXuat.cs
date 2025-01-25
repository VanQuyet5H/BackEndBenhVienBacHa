using Camino.Core.Domain.Entities.NhaSanXuatTheoQuocGias;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NhaSanXuats
{
    public class NhaSanXuat : BaseEntity
    {
        public string Ten { get; set; }
        public string Ma { get; set; }

        private ICollection<NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGia> _nhaSanXuatTheoQuocGias;
        public virtual ICollection<NhaSanXuatTheoQuocGias.NhaSanXuatTheoQuocGia> NhaSanXuatTheoQuocGias
        {
            get => _nhaSanXuatTheoQuocGias ?? (_nhaSanXuatTheoQuocGias = new List<NhaSanXuatTheoQuocGia>());
            protected set => _nhaSanXuatTheoQuocGias = value;
        }
    }
}
