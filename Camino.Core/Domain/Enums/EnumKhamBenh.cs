using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Camino.Core.Domain
{
    public partial class Enums
    {
        public enum KhamBenhDichVuKhamVietTat
        {
            [Description("CauHinhKhamBenh.DichVuKhamNoi")]
            DichVuKhamNoi = 1,
            [Description("CauHinhKhamBenh.DichVuKhamCapCuu")]
            DichVuKhamCapCuu = 2,
            [Description("CauHinhKhamBenh.DichVuKhamThai")]
            DichVuKhamThai = 3,
        }
    }
}
