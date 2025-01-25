using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TongHopYLenh
{
    public class TongHopYLenhDienBienChiTietViewModel : BaseViewModel
    {
        public long? NoiTruPhieuDieuTriId { get; set; }
        public string MoTaYLenh { get; set; }
        public long? NhanVienXacNhanThucHienId { get; set; }
        public int? GioThucHien { get; set; }
        public bool? XacNhanThucHien { get; set; }
        public long? NhanVienCapNhatId { get; set; }
        public string NhanVienCapNhatDisplay { get; set; }
        public DateTime? ThoiDiemCapNhat { get; set; }
        public string ThoiDiemCapNhatDisplay { get; set; }
        public long? NhanVienChiDinhId { get; set; }
        public string NhanVienChiDinhDisplay { get; set; }
        public long? NoiChiDinhId { get; set; }

        //Cập nhật 06/06/2022: Cập nhật giờ thực hiện -> đổi từ chỉ nhập giờ thành nhập ngày giờ (Áp dụng cho trường hợp check vào xác nhận thực hiện trên grid)
        public DateTime? ThoiDiemXacNhanThucHien { get; set; }
    }
}
