using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.PhauThuatThuThuats
{
    public class PhauThuatThuThuatEkipDieuDuong : BaseEntity
    {
        public long YeuCauDichVuKyThuatTuongTrinhPTTTId { get; set; }
        public long NhanVienId { get; set; }
        public EnumVaiTroDieuDuong? VaiTroDieuDuong { get; set; }

        public virtual YeuCauDichVuKyThuatTuongTrinhPTTT YeuCauDichVuKyThuatTuongTrinhPTTT { get; set; }
        public virtual NhanVien NhanVien { get; set; }
    }
}
