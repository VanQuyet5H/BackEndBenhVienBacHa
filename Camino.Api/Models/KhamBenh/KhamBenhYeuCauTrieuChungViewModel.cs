using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauTrieuChungViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long TrieuChungId { get; set; }

        //public virtual TrieuChung TrieuChung { get; set; }
        //public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
    }
}
