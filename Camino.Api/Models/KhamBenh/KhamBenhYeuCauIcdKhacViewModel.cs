using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauIcdKhacViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long ICDId { get; set; }

        //public virtual ICD Icd { get; set; }
        //public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
    }
}
