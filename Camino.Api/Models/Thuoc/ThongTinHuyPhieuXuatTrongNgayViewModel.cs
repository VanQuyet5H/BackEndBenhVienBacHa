using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.Thuoc
{
    public class ThongTinHuyPhieuXuatTrongNgayViewModel : BaseViewModel
    {
        public long TaiKhoanBenhNhanThuId { get; set; }
        public bool? ThuHoiPhieu { get; set; }
        public int? NguoiThuHoiId { get; set; }
        public string TenNguoiThuHoi { get; set; }
        public DateTime? ThoiGianThuHoi { get; set; }
        public string LyDo { get; set; }
    }
}
