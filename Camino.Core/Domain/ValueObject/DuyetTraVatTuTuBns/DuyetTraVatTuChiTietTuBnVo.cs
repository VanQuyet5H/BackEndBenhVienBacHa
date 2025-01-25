using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.DuyetTraVatTuTuBns
{
    public class DuyetTraVatTuChiTietTuBnVo : GridItem
    {
        public long? YeuCauTraVatTuTuBenhNhanId { get; set; }

        public long? VatTuBenhVienId { get; set; }

        public string VatTu { get; set; }

        public bool LaVatTuBhyt { get; set; }

        public string Nhom => LaVatTuBhyt ? "BHYT" : "Không BHYT";

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
