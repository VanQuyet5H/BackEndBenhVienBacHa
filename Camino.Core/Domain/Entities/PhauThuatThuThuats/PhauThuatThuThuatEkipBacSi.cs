using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.PhauThuatThuThuats
{
    public class PhauThuatThuThuatEkipBacSi : BaseEntity
    {
        public long YeuCauDichVuKyThuatTuongTrinhPTTTId { get; set; }
        public long NhanVienId { get; set; }
        public EnumVaiTroBacSi? VaiTroBacSi { get; set; }

        public virtual YeuCauDichVuKyThuatTuongTrinhPTTT YeuCauDichVuKyThuatTuongTrinhPTTT { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}
