using System;

namespace Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns
{
    public class CheckApproveVo
    {
        public bool DangChoDuyet { get; set; }

        public bool DaDuyet { get; set; }

        public string SearchString { get; set; }

        public RangeDates RangeYeuCau { get; set; }

        public RangeDates RangeDuyet { get; set; }
    }

    public class RangeDates
    {
        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public string TuNgay { get; set; }

        public string DenNgay { get; set; }
    }
}
