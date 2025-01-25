using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.KhamBenh.ViewModelCheckValidators
{
    public class ChuyenKhamYeuCauKhamBenhViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauKhamBenhTruocId { get; set; }
        public long? PhongBenhVienHangDoiTruocId { get; set; }
        public long? DichVuKhamBenhBenhVienId { get; set; }
        public int? SoLuong { get; set; }
        public long? NoiDangKyId { get; set; }
        public long? BacSiDangKyId { get; set; }
        public bool? IsKhamBenhDangKham { get; set; }
    }
    public class CapNhatPhieuNghiDuongThai
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public long YeuCauKhamBenhId { get; set; }
    }
    public class InPhieuNghiDuongThaiQueryInfo
    {
        public long YeuCauKhamBenhId { get; set; }
    }
}
