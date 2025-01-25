using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThongTinMienGiamVoucherVo
    {
        public Enums.LoaiVoucher LoaiVoucher { get; set; }
        public List<LookupItemVo> MaVouchers { get; set; }
        public string TenVoucher { get; set; }

        public List<DichVuMiemGiamTheoTiLe> DichVuMiemGiamTheoTiLes { get; set; }

        public List<DuocPhamMienGiamTheoTiLe> DuocPhamMienGiamTheoTiLes { get; set; }

        public decimal SoTienVoucherMiemGiam { get; set; }
    }
}
