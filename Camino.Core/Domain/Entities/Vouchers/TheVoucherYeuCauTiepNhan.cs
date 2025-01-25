using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DoiTuongUuDais;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.Vouchers
{
    public class TheVoucherYeuCauTiepNhan : BaseEntity
    {
        public long TheVoucherId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public long BenhNhanId { get; set; }

        public virtual TheVoucher TheVoucher { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
    }
}
