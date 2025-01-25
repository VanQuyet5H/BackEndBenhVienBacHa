using System;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BenhNhans
{
    public class BenhNhanTienSuBenhGridVo : GridItem
    {
        public long BenhNhanId { get; set; }

        public long ICDId { get; set; }

        public DateTime? NgayPhatHien { get; set; }

        public string NgayPhatHienDisplay { get; set; }

        public string TinhTrangBenh { get; set; }
    }

    public class BenhNhanTienSuKhamBenhGridVo : GridItem
    {
        public long BenhId { get; set; }
        public string TenBenh { get; set; }
        public string LoaiTienSuBenh { get; set; }
        public string NgayPhatHien { get; set; }

        public string TenTinhTrang { get; set; }
        public long BenhNhanId { get; set; }

        public long ICDId { get; set; }

        public string NgayPhatHienDisplay { get; set; }

        public string TinhTrangBenh { get; set; }
    }
}
