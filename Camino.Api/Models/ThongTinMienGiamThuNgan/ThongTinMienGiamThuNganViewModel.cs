using Camino.Core.Domain;

namespace Camino.Api.Models.ThongTinMienGiamThuNgan
{
    public class ThongTinMienGiamThuNganViewModel
    {
        public bool ValidateVoucher { get; set; }

        public bool ValidateMGThem { get; set; }

        public long IdYeuCauTiepNhan { get; set; }

        public long BenhNhanId { get; set; }

        public long[] ListVouchers { get; set; }

        public decimal? SoTienMG { get; set; }

        public int? TiLeMienGiam { get; set; }

        public decimal SoTienMGConLai { get; set; }

        public string LyDoMienGiam { get; set; }

        //public Enums.LoaiMienGiamThem LoaiMienGiamThem { get; set; }

        public TaiLieuDinhKemGiayMiemGiamViewModel TaiLieuDinhKem { get; set; }

        public long? NhanVienDuyetMienGiamThemId { get; set; }
    }

    public class ThongTinChinhSachMiemGiamThem
    {
        public long IdYeuCauTiepNhan { get; set; }

        public string LyDoMienGiam { get; set; }

        public TaiLieuDinhKemGiayMiemGiamViewModel TaiLieuDinhKem { get; set; }

        public long? NhanVienDuyetMienGiamThemId { get; set; }
    }

    public class TaiLieuDinhKemGiayMiemGiamViewModel
    {
        public string Ma { get; set; }

        public string Ten { get; set; }

        public string TenGuid { get; set; }

        public string DuongDan { get; set; }

        public long? KichThuoc { get; set; }

        public int? LoaiTapTin { get; set; }

        public string MoTa { get; set; }
    }
}
