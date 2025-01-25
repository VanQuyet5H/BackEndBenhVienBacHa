using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.BenhNhans
{
    public class BenhNhanViewModel : BaseViewModel
    {
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhHienThi { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }
        public string DiaChiDayDu { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public decimal SoDuTaiKhoan { get; set; }

        //BVHD-3941
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }
}
