using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum ThoiGianMacDinhTrongNgay
        {
            [Description("Sáng")]
            Sang = 1,
            [Description("Trưa")]
            Trua = 2,
            [Description("Chiều")]
            Chieu = 3,
            [Description("Tối")]
            Toi = 4
        }
    }
}
