using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.CongNoBenhNhans
{
    public class ThongTinThuNoGridVo : GridItem
    {
        public string SoPhieuThu { get; set; }
        public long SoPhieuNoId { get; set; }
        public string SoPhieuNo { get; set; }
        public string MaTiepNhan { get; set; }
        public DateTime Ngay { get; set; }
        public string NgayDisplay => Ngay.ApplyFormatDate();
        public decimal SoTienThu { get; set; }
    }
}
