using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;

namespace Camino.Core.Domain.Entities.ChiSoXetNghiems
{
    public class KetQuaXetNghiem : BaseEntity
    {
        public long ChiSoXetNghiemId { get; set; }
        public long YeuCauDichVuKyThuatId { get; set; }
        public string MaMay { get; set; }
        public string KetQua { get; set; }
        public string MoTa { get; set; }
        public string KetLuan { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }

        public virtual ChiSoXetNghiem ChiSoXetNghiem { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
    }
}
