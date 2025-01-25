using System.Collections;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.CauHinhs
{
    public class CauHinhTheoThoiGian : BaseEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Enums.DataType DataType { get; set; }

        private ICollection<CauHinhTheoThoiGianChiTiet> _cauHinhTheoThoiGianChiTiets;

        public virtual ICollection<CauHinhTheoThoiGianChiTiet> CauHinhTheoThoiGianChiTiets
        {
            get => _cauHinhTheoThoiGianChiTiets ??
                   (_cauHinhTheoThoiGianChiTiets = new List<CauHinhTheoThoiGianChiTiet>());
            protected set => _cauHinhTheoThoiGianChiTiets = value;
        }
    }
}
