using Camino.Api.Models.ICDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauChuanDoanViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long ChuanDoanId { get; set; }

        public virtual ChuanDoanViewModel ChuanDoan { get; set; }
        public virtual KhamBenhYeuCauTiepNhanViewModel YeuCauTiepNhan { get; set; }
    }
}
