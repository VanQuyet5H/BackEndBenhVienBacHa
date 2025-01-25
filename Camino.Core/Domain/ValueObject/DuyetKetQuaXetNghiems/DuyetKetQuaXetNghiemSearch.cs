using Camino.Core.Domain.ValueObject.YeuCauMuaDuocPham;
using System;

namespace Camino.Core.Domain.ValueObject.DuyetKetQuaXetNghiems
{
    public class DuyetKetQuaXetNghiemSearch
    {
        public bool DangThucHien { get; set; }
        public bool ChoDuyet { get; set; }
        
        public bool DaDuyet { get; set; }
        
        public string SearchString { get; set; }
        
        public RangeDates RangeThucHien { get; set; }
        
        public RangeDate RangeDuyet { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

    }

    public class RangeDate
    {
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
    }
}
