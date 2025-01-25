using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class GiayCamKetSuDungKyThuatMoiHSViewModel
    {
        public GiayCamKetSuDungKyThuatMoiHSViewModel()
        {
            FileChuKy = new List<FileChuKyViewModel>();
        }

        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public string QuanHe { get; set; }

        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string ChanDoan { get; set; }


        public double? SoTien { get; set; }

        public string SoTienChu { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string BacSyThucHien { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }
        public bool? CheckCreateOrCapNhat { get; set; }

        public List<FileChuKyViewModel> FileChuKy { get; set; }
    }
}
