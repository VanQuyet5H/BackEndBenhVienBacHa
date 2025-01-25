using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.VatTu
{
    public class NhapKhoVatTuTonKhoViewModel : BaseViewModel
    {
        public NhapKhoVatTuTonKhoViewModel()
        {
            NhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTietTonKhoViewModel>();
        }
        public long? VatTuId { get; set; }
        public List<NhapKhoVatTuChiTietTonKhoViewModel> NhapKhoVatTuChiTiets { get; set; }
    }
}
