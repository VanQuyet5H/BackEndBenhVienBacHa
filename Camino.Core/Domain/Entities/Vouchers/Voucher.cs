using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.Vouchers
{
    public class Voucher : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public int SoLuongPhatHanh { get; set; }
        public string MoTa { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public bool? ChietKhauTatCaDichVu { get; set; }
        public Enums.LoaiChietKhau? LoaiChietKhau { get; set; }
        public int? TiLeChietKhau { get; set; }
        public decimal? SoTienChietKhau { get; set; }

        private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        {
            get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
            protected set => _voucherChiTietMienGiams = value;
        }

        private ICollection<TheVoucher> _theVouchers { get; set; }
        public virtual ICollection<TheVoucher> TheVouchers
        {
            get => _theVouchers ?? (_theVouchers = new List<TheVoucher>());
            protected set => _theVouchers = value;
        }
    }
}
