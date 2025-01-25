using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class ThongTinCLS : GridItem
    {
        public string NoiDung { get; set; }
        public string NguoiThucHien { get; set; }
        public DateTime NgayThucHien { get; set; }
        public string NgayThucHienDisplay => NgayThucHien.ApplyFormatDateTimeSACH();
        public string NguoiKetLuan { get; set; }
        public DateTime NgayKetLuan { get; set; }
        public string NgayKetLuanDisplay => NgayKetLuan.ApplyFormatDateTimeSACH();
        public string ChanDoan { get; set; }
        public string XemKQ { get; set; }
        public bool isCheck { get; set; }
    }
}
