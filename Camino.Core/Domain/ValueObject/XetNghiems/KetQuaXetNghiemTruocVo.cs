using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.XetNghiems
{
    public class KetQuaXetNghiemTruocVo
    {
        public DateTime? ThoiDiemKetLuan { get; set; }
        public long DichVuXetNghiemId { get; set; }
        public string GiaTriTuMay { get; set; }
        public string GiaTriNhapTay { get; set; }
        public string GiaTriDuyet { get; set; }
    }
}
