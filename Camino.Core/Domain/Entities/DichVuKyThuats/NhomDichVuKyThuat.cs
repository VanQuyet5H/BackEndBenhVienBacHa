using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DichVuKyThuats
{
    public class NhomDichVuKyThuat: BaseEntity
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long? NhomDichVuKyThuatChaId { get; set; }
        public int CapNhom { get; set; }


        private ICollection<DichVuKyThuat> _dichVuKyThuats { get; set; }
        public virtual ICollection<DichVuKyThuat> DichVuKyThuats
        {
            get => _dichVuKyThuats ?? (_dichVuKyThuats = new List<DichVuKyThuat>());
            protected set => _dichVuKyThuats = value;
        }
       
    }
}
