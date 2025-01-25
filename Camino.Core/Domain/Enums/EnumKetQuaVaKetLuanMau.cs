using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum LoaiKetQuaVaKetLuanMau
        {
            [Description("Kết quả mẫu")]
            KetQuaMau = 1,
            [Description("Kết luận mẫu")]
            KetLuanMau = 2
        }
    }
}
