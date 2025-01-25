using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuNghiDuongThai
{
    public class NgayDuongThaiYeuCauKhamBenhVo
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public long YeuCauKhamBenhId { get; set; }
    }
}
