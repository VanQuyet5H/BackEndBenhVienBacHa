using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NhomDichVuKyThuat
{
    public class NhomDichVuKyThuatViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long? NhomDichVuKyThuatChaId { get; set; }
        public int CapNhom { get; set; } 
    }
}
