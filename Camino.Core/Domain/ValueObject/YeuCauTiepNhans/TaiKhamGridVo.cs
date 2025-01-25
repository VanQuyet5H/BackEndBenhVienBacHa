using System;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class TaiKhamGridVo : GridItem
    {
        public string TenDichVu { get; set; }
        public string BacSiThucHien { get; set; }
        public string NgayTaiKhamDisplay { get; set; }
        public DateTime NgayTaiKham { get; set; }
        public string GhiChuTaiKham { get; set; }
    }
}