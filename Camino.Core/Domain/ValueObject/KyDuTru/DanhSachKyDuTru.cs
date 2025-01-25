using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KyDuTru
{
    public class KyDuTruSearch
    {
        public bool DuocPham { get; set; }
        public bool VatTu { get; set; }
        public string SearchString { get; set; }
        public RangeDate RangeApDung { get; set; }
        public RangeDate RangeLap { get; set; }
    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
