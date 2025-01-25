using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;

namespace Camino.Core.Domain.Entities.Vouchers
{
    public class TheVoucher : BaseEntity
    {
        public string Ma { get; set; }
        public long VoucherId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public decimal? TongGiaTriDaSuDung { get; set; }

        public virtual Voucher Voucher { get; set; }

        private ICollection<TheVoucherYeuCauTiepNhan> _theVoucherYeuCauTiepNhans { get; set; }
        public virtual ICollection<TheVoucherYeuCauTiepNhan> TheVoucherYeuCauTiepNhans
        {
            get => _theVoucherYeuCauTiepNhans ?? (_theVoucherYeuCauTiepNhans = new List<TheVoucherYeuCauTiepNhan>());
            protected set => _theVoucherYeuCauTiepNhans = value;
        }

        private ICollection<MienGiamChiPhi> _mienGiamChiPhis;
        public virtual ICollection<MienGiamChiPhi> MienGiamChiPhis
        {
            get => _mienGiamChiPhis ?? (_mienGiamChiPhis = new List<MienGiamChiPhi>());
            protected set => _mienGiamChiPhis = value;
        }
    }
}
