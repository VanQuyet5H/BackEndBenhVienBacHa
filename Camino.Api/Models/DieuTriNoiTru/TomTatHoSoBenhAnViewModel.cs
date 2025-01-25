using System;
using System.Collections.Generic;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class TomTatHoSoBenhAnViewModel
    {
        public TomTatHoSoBenhAnViewModel()
        {
            FileChuKy = new List<FileChuKyViewModel>();
        }
        public string DienBienLamSang { get; set; }

        public string KqXnCls { get; set; }

        public string PpDieuTri { get; set; }

        public string TinhTrangChuyenTuyen { get; set; }

        public string GhiChu { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string GiamDoc { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }

        public bool IsCreated { get; set; }

        public List<FileChuKyViewModel> FileChuKy { get; set; }
    }
}
