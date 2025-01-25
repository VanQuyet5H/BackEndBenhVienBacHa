using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DichVuKyThuatBenhVien
{
    public class DuocPhamBenhVienVacxinViewModel
    {
        public string SoDangKy { get; set; }
        public string MaDuocPhamBenhVien { get; set; }
        public string Ten { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string QuyCach { get; set; }
    }
}
