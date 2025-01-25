using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuyetTraThuocTuBns
{
    public class DuyetTraThuocChiTietTuBnVo : GridItem
    {
        public long? YeuCauTraDuocPhamTuBenhNhanId { get; set; }

        public long? DuocPhamBenhVienId { get; set; }

        public string DuocPham { get; set; }

        public bool LaDuocPhamBhyt { get; set; }

        public string Nhom => LaDuocPhamBhyt ? "BHYT" : "Không BHYT";

        public string HoatChat { get; set; }

        public string Dvt { get; set; }

        public double? TongSlChiDinhSetting { get; set; }

        public double? TongSlDaTraSetting { get; set; }

        public double? TongSlTraLanNaySetting { get; set; }

        public string TongSlChiDinh => TongSlChiDinhSetting != null && TongSlChiDinhSetting != 0 ? TongSlChiDinhSetting.ApplyNumber() : "0";

        public string TongSlDaTra => TongSlDaTraSetting != null && TongSlDaTraSetting != 0 ? TongSlDaTraSetting.ApplyNumber() : "0";

        public string TongSlTraLanNay => TongSlTraLanNaySetting != null && TongSlTraLanNaySetting != 0 ? TongSlTraLanNaySetting.ApplyNumber() : "0";
        public bool? DuocDuyet { get; set; }
    }
}
