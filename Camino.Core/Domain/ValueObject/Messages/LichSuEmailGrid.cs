using System;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.Messages
{
    public class LichSuEmailGrid: GridItem
    {
        public string GoiDen { get; set; }
        public string NoiDung { get; set; }
        public string TieuDe { get; set; }
        public string TapTinDinhKem { get; set; }
        public Enums.TrangThaiLishSu TrangThai { get; set; }
        public string TenTrangThai { get; set; }
        public string urlTapTinDinhKem { get; set; }
        public DateTime? NgayGui { get; set; }
        public string NgayGuiFormat { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }
}
