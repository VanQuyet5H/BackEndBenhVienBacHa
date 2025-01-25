using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenhChanDoanPhanBiet: BaseEntity
    {
        public long ICDId { get; set; }
        public string GhiChu { get; set; }
        public long YeuCauKhamBenhId { get; set; }

        public virtual ICD ICD { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
    }
    
}
