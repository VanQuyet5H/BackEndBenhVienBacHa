using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NguoiGioiThieu
{
    public class NguoiGioiThieuViewModel : BaseViewModel
    {
        public string HoTen { get; set; }
        public string SoDienThoai { get; set; }
        public long? NhanVienQuanLyId { get; set; }
        public string HoTenNguoiQuanLy { get; set; }
        public string SoDienThoaiDisplay { get; set; }

    }
}
