using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.GayBenhAn
{
    public class GayBenhAnViewModel : BaseViewModel
    {
        public GayBenhAnViewModel()
        {
            GayBenhAnPhieuHoSoIds = new List<string>();
        }
        public string Ma { get; set; }
        public int? ViTriGay { get; set; }
        public string Ten { get; set; }
        public bool? IsDisabled { get; set; }
        public List<string> GayBenhAnPhieuHoSoIds { get; set; }
    }

}
