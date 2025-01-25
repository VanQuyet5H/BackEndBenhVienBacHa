using System;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class DateTimeFilterVo
    {
        // filter thường
        public RangeDateFilterVo RangeDateTimeFilter { get; set; }

        // filter kỳ so sánh
        public RangeDateFilterVo RangeDateTimeSoSanh { get; set; }
    }

    public class RangeDateFilterVo
    {
        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }
    }
}
