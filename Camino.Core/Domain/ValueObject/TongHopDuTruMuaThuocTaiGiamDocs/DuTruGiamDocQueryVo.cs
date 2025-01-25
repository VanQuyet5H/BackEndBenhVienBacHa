using System;

namespace Camino.Core.Domain.ValueObject.TongHopDuTruMuaThuocTaiGiamDocs
{
    public class DuTruGiamDocQueryVo
    {
        public bool DangChoDuyet { get; set; }
        public bool TuChoiDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeYeuCau { get; set; }
        public RangeDate RangeDuyet { get; set; }
    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
