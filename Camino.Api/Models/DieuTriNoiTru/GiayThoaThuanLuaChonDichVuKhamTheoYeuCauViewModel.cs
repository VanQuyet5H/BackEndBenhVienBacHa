using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DieuTriNoiTru
{
   
    public class GiayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel
    {
        public GiayThoaThuanLuaChonDichVuKhamTheoYeuCauViewModel()
        {
            FileChuKy = new List<FileChuKyViewModel>();
            BacSiKhams = new List<long>();
        }

        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public string QuanHe { get; set; }

        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string KhiCanBaoTin { get; set; }
        

        public string BacSiKham { get; set; }
        public string NamTaiBuongLoai { get; set; }

        public double? SoTien { get; set; }

        public string SoTienChu { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string BsGmhs { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }
        public bool? CheckCreateOrCapNhat { get; set; }

        public List<FileChuKyViewModel> FileChuKy { get; set; }
        public List<long >BacSiKhams { get; set; }
    }
}
