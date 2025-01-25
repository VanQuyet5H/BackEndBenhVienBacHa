using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenhKhamBoPhanKhac:BaseEntity
    {
        public string Ten { get; set; }
        public string NoiDUng { get; set; }
        public long YeuCauKhamBenhId { get; set; }

        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
    }
}
