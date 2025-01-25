using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.XacNhanBhytDaHoanThanh
{
    public class DuyetBaoHiemHoanThanhGridVo : GridItem
    {
        public string NguoiXacNhan { get; set; }

        public DateTime ThoiDiemDuyet { get; set; }

        public string NgayXacNhan => ThoiDiemDuyet.ApplyFormatDateTimeSACH();
    }
}
