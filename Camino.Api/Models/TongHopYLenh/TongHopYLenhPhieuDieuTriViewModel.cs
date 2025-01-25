using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TongHopYLenh
{
    public class TongHopYLenhPhieuDieuTriViewModel
    {
        public TongHopYLenhPhieuDieuTriViewModel()
        {
            TongHopYLenhDienBiens = new List<TongHopYLenhDienBienViewModel>();
        }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public DateTime? NgayYLenh { get; set; }

        public List<TongHopYLenhDienBienViewModel> TongHopYLenhDienBiens { get; set; }
    }
}
