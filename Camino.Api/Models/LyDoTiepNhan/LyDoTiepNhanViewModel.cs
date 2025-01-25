using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.LyDoTiepNhan
{
    public class LyDoTiepNhanViewModel : BaseViewModel
    {
        public LyDoTiepNhanViewModel()
        {
            LyDoTiepNhans = new List<LyDoTiepNhanViewModel>();
        }

        public string Ten { get; set; }
        public string TenCha { get; set; }
        public long? LyDoTiepNhanChaId { get; set; }
        public int? CapNhom { get; set; }
        public string MoTa { get; set; }
        public List<LyDoTiepNhanViewModel> LyDoTiepNhans { get; set; }
    }
}
