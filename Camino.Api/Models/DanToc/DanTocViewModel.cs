using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Api.Models.KhamBenh;

namespace Camino.Api.Models.DanToc
{
    public class DanTocViewModel : BaseViewModel
    {
        public DanTocViewModel()
        {
            YeuCauTiepNhans = new List<KhamBenhYeuCauTiepNhanViewModel>();
        }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenQuocGia { get; set; }
        public int QuocGiaId { get; set; }
        public bool? IsDisabled { get; set; }

        public List<KhamBenhYeuCauTiepNhanViewModel> YeuCauTiepNhans { get; set; }
    }
}
