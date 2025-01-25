using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.Cauhinh
{
    public class CauHinhNguoiDuyetTheoNhomDichVuViewModel:BaseViewModel
    {
        public long? NhanVienId { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
    }
}
