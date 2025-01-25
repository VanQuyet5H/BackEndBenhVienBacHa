using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.CongNoBenhNhans
{
    public class ThongTinChuaThuNoGridVo : GridItem
    {
        public string SoPhieuNo { get; set; }
        public string MaTiepNhan { get; set; }
        public DateTime Ngay { get; set; }
        public string NgayDisplay => Ngay.ApplyFormatDate();
        public decimal SoTienCongNo { get; set; }
        public decimal SoTienDaThu { get; set; }
        public decimal SoTienChuaThu => SoTienCongNo - SoTienDaThu;
    }
}
