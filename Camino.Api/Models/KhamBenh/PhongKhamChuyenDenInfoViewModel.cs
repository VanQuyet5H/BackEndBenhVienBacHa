using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh
{
    public class PhongKhamChuyenDenInfoViewModel
    {
        public PhongKhamChuyenDenInfoViewModel()
        {
            HangDoiIds = new List<long>();
        }
        public List<long> HangDoiIds { get; set; }
        public long PhongHienTaiId { get; set; }
        public long? PhongThucHienId { get; set; }
    }
}
