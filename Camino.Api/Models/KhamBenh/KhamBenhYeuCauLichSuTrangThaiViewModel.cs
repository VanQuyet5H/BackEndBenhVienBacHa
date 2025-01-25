 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class KhamBenhYeuCauLichSuTrangThaiViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public int TrangThaiYeuCauTiepNhan { get; set; }
        public string MoTa { get; set; }
        //public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
    }
}
