using System;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.Messages
{
    public class LichSuThongBaoGripVo : GridItem
    {
        public string GoiDen { get; set; }
        public string NoiDung { get; set; }
        public string NgayGui { get; set; }
        public string TenTrangThai { get; set; }
        public Enums.TrangThaiLishSu? TrangThai { get; set; }

        public DateTime? NgayGuiDate { get; set; }
        public DateTime? NgayGuiTu { get; set; }
        public DateTime? NgayGuiDen { get; set; }
    }
}
