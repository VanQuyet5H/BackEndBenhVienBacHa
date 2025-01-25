using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum EnumNhomVatTu
        {
            [Description("Nhóm KSNK")]
            NhomKSNK = 44,

            [Description("Nhóm Hành chính")]
            NhomHanhChinh = 45,
        }
    }
}
