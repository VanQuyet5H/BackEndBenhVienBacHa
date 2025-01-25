using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThongTinVoucherVo : LookupItemVo
    {
        public int LoaiVoucher { get; set; }
    }
    public class ThongTinVoucherTheoYeuCauTiepNhan
    {
        public string TheVoucher { get; set; }
        public long YeucauTiepNhanId { get; set; }
    }
}
