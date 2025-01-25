using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TongHopYLenh
{
    public class TongHopYLenhDienBienViewModel
    {
        public TongHopYLenhDienBienViewModel()
        {
            TongHopYLenhDienBienChiTiets = new List<TongHopYLenhDienBienChiTietViewModel>();
        }
        public int? GioYLenh { get; set; }
        public string GioYLenhDisplay { get; set; }
        public string DienBien { get; set; }

        public List<TongHopYLenhDienBienChiTietViewModel> TongHopYLenhDienBienChiTiets { get; set; }
    }
}
