using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BHYT
{
    public class HoSoCanLamSangViewModel : BaseViewModel
    {
        public string MaLienKet { get; set; }
        public string MaDichVu { get; set; }
        public string MaChiSo { get; set; }
        public string TenChiSo { get; set; }
        public string GiaTri { get; set; }
        public string MaMayXetNghiem { get; set; }
        public string MaMay { get; set; }
        public string MoTa { get; set; }
        public string KetLuan { get; set; }
        public string NgayKQ { get; set; }
        public DateTime? NgayKQTime { get; set; }
    }
}
