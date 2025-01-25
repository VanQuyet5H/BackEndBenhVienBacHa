using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.YeuCauTiepNhan;
using System;

namespace Camino.Api.Models.ChiSoXetNghiems
{
    public class KetQuaXetNghiemViewModel : BaseViewModel
    {
        public long? ChiSoXetNghiemId { get; set; }
        public long? YeuCauTiepNhanDichVuKyThuatId { get; set; }
        public string MaMay { get; set; }
        public string KetQua { get; set; }
        public string MoTa { get; set; }
        public string KetLuan { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }

        public virtual ChiSoXetNghiemViewModel ChiSoXetNghiemViewModel { get; set; }
        public virtual KhamBenhYeuCauDichVuKyThuatViewModel YeuCauKhamBenhDichVuKyThuatViewModel { get; set; }
    }
}
