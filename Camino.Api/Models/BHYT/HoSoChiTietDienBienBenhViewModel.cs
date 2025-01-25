using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT
{
    public class HoSoChiTietDienBienBenhViewModel : BaseViewModel
    {
        public string MaLienKet { get; set; }
        public string DienBien { get; set; }
        public string HoiChuan { get; set; }
        public string PhauThuat { get; set; }
        public string NgayYLenh { get; set; }
        public DateTime? NgayYLenhTime { get; set; }
    }
}
