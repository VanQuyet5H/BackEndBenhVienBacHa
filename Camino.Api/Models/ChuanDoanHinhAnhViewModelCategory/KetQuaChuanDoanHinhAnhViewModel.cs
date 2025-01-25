using Camino.Api.Models.KhamBenh;
using Camino.Api.Models.YeuCauTiepNhan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.ChuanDoanHinhAnhViewModelCategory
{
    public class KetQuaChuanDoanHinhAnhViewModel : BaseViewModel
    {
        public long? ChuanDoanHinhAnhId { get; set; }
        public long? YeuCauTiepNhanDichVuKyThuatId { get; set; }
        public string MaMay { get; set; }
        public string KetQua { get; set; }
        public string MoTa { get; set; }
        public string KetLuan { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }

        public ChuanDoanHinhAnhViewModel ChuanDoanHinhAnhViewModel { get; set; }
        //public KBYeuCauTiepNhanDichVuKyThuatViewModel YeuCauTiepNhanDichVuKyThuatViewModel { get; set; }
    }
}
