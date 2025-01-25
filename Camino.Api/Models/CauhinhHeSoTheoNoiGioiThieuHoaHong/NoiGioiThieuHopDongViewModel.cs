using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.CauhinhHeSoTheoNoiGioiThieuHoaHong
{
    public class NoiGioiThieuHopDongViewModel :BaseViewModel
    {
        public long NoiGioiThieuId { get; set; }
        public string Ten { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
    }
}
