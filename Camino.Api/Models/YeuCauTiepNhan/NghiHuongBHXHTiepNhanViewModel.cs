using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.YeuCauTiepNhan
{
    public class NghiHuongBHXHTiepNhanViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemTiepNhan { get; set; }
        public DateTime? DenNgay { get; set; }
        public long? BacSiKetLuanId { get; set; }
    }
}
