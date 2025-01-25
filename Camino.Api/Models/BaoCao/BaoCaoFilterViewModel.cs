using System;

namespace Camino.Api.Models.BaoCao
{
    public class BaoCaoFilterViewModel
    {
        // filter thường
        public DateTimeFilter RangeDateTimeFilter { get; set; }

        // filter kỳ so sánh
        public DateTimeFilter RangeDateTimeSoSanh { get; set; }

        //// filter thường
        //public DateTimeFilter RangeDateTimeFilter = new DateTimeFilter();

        //// filter kỳ so sánh
        //public DateTimeFilter RangeDateTimeSoSanh = new DateTimeFilter();
    }

    public class DateTimeFilter
    {
        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }
    }
}
