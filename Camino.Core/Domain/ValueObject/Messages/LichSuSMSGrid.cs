using System;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.Messages
{
   public class LichSuSMSGrid : GridItem
    {
        public string GoiDen { get; set; }
        public string NoiDung { get; set; }
        public string TenTrangThai { get; set; }
        public string NgayGui { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
        public DateTime Ngay { get; set; }
        public Enums.TrangThaiLishSu TrangThai { get; set; }
    }

    
}
