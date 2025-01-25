using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThongTinMienGiamVo
    {
        public ThongTinMienGiamTheoDoiTuongUuDaiVo ThongTinMienGiamTheoDoiTuongUuDaiVo { get; set; }
        public ThongTinMienGiamVoucherVo ThongTinMienGiamVoucherVo { get; set; }
        public ThongTinMienGiamThemVo ThongTinMienGiamThemVo { get; set; }
        public bool KhachVangLaiMuaThuoc { get; set; }
    }

    public class DichVuMiemGiamTheoTiLe
    {
        public NhomChiPhiKhamChuaBenh LoaiNhom { get; set; }
        public long DichVuId { get; set; }
        public int TiLe { get; set; }
    }

    public class DuocPhamMienGiamTheoTiLe
    {
        public long DuocPhamId { get; set; }

        public int TiLe { get; set; }
    }
}
