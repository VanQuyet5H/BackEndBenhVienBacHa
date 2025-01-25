using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.NoiGioiThieu
{
    public class NoiGioiThieuViewModel : BaseViewModel
    {
        public NoiGioiThieuViewModel()
        {
            NoiGioiThieuChiTietMienGiams = new List<NoiGioiThieuChiTietMienGiamViewModel>();
        }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public string SoDienThoai { get; set; }
        public string DonVi { get; set; }
        public long? DonViMauId { get; set; }
        public string TenDonViMau { get; set; }
        public long? NhanVienQuanLyId { get; set; }
        public string HoTenNguoiQuanLy { get; set; }
        public bool? IsDisabled { get; set; }
        public string SoDienThoaiDisplay { get; set; }

        public List<NoiGioiThieuChiTietMienGiamViewModel> NoiGioiThieuChiTietMienGiams { get; set; }
    }

    public class DonViMauViewModel : BaseViewModel
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public bool IsDefault { get; set; }
    }
}
