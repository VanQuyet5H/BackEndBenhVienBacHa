using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TiemChung
{
    public class ThucHienTiemKhamSangLocViewModel : BaseViewModel
    {
        public ThucHienTiemKhamSangLocViewModel()
        {
            YeuCauDichVuKyThuats = new List<ThucHienTiemVacxinViewModel>();
        }
        public long? NoiTheoDoiSauTiemId { get; set; }
        public bool? IsHoanThanhTiem { get; set; }

        public List<ThucHienTiemVacxinViewModel> YeuCauDichVuKyThuats { get; set; }
    }
}
