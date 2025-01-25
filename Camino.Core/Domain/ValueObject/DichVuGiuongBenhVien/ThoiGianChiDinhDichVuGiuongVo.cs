using System;

namespace Camino.Core.Domain.ValueObject.DichVuGiuongBenhVien
{
    public class ThoiGianChiDinhDichVuGiuongVo
    {
        public ThoiGianChiDinhDichVuGiuongVo()
        {
            ThoiGianNhan = DateTime.Now;
            //ThoiGianTra = DateTime.Now;
        }

        public DateTime? ThoiGianNhan { get; set; }
        //public DateTime? ThoiGianTra { get; set; }
    }
}
