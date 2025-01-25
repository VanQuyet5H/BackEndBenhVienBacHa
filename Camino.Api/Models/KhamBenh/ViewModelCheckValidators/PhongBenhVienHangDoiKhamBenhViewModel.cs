using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain;

namespace Camino.Api.Models.KhamBenh.ViewModelCheckValidators
{
    public class PhongBenhVienHangDoiKhamBenhViewModel: BaseViewModel
    {
        public long PhongBenhVienId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }

        public virtual YeuCauTiepNhanKhamBenhViewModel YeuCauTiepNhan { get; set; }
        public virtual YeuCauKhamBenhKhamBenhViewModel YeuCauKhamBenh { get; set; }
    }
}
