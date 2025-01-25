using Camino.Core.Domain.Entities.DichVuXetNghiems;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.HDPP
{
    public class HDPP : BaseEntity
    {
        public string Ten { get; set; }

        private ICollection<DichVuXetNghiem> _dichVuXetNghiems;
        public virtual ICollection<DichVuXetNghiem> DichVuXetNghiems
        {
            get => _dichVuXetNghiems ?? (_dichVuXetNghiems = new List<DichVuXetNghiem>());
            protected set => _dichVuXetNghiems = value;
        }
    }
}
