using System;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo
    {
        public KhamDoanYeuCauNhanSuKhamSucKhoeQueryVo()
        {
            TrangThai = new TrangThai();
        }
        public string SearchString { get; set; }
        public TrangThai TrangThai { get; set; }
        public RangeDate NgayGui { get; set; }
        public RangeDate NgayKHTHDuyet { get; set; }
        public RangeDate NgayNhanSuDuyet { get; set; }
        public RangeDate NgayGiamDocDuyet { get; set; }
    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }

    public class TrangThai
    {
        public bool ChoDuyet { get; set; }
        public bool DaDuyet { get; set; }
        public bool TuChoi { get; set; }
    }
}
