using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.ThongTinCongTyBaoHiemTuNhan
{
    public class ThongTinCongTyBaoHiemTuNhanVo: GridItem
    {
        public long CongNoId { get; set; }

        public string TenCongTy { get; set; }
    }
}
