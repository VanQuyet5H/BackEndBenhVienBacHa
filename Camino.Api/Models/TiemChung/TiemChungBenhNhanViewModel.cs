using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungBenhNhanViewModel : BaseViewModel
    {
        public TiemChungBenhNhanViewModel()
        {
            BenhNhanTiemVacxinDiUngThuocs = new List<TiemChungBenhNhanDiUngThuocViewModel>();
            BenhNhanTiemVacxinTienSuBenhs = new List<TiemChungBenhNhanTienSuBenhViewModel>();
            YeuCauTiepNhans = new List<TiemChungYeuCauTiepNhanViewModel>();
        }
        public string MaBN { get; set; }

        public List<TiemChungBenhNhanDiUngThuocViewModel> BenhNhanTiemVacxinDiUngThuocs { get; set; }
        public List<TiemChungBenhNhanTienSuBenhViewModel> BenhNhanTiemVacxinTienSuBenhs { get; set; }
        public List<TiemChungYeuCauTiepNhanViewModel> YeuCauTiepNhans { get; set; }
    }
}
