using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TonKho
{
    public class CapNhatTonKhoDuocPhamViewModel : BaseViewModel
    {
        public CapNhatTonKhoDuocPhamViewModel()
        {
            CapNhatTonKhoDuocPhamChiTiets = new List<CapNhatTonKhoDuocPhamChiTietViewModel>();
        }
        public long? DuocPhamId { get; set; }
        public string SoDangKy { get; set; }
        public List<CapNhatTonKhoDuocPhamChiTietViewModel> CapNhatTonKhoDuocPhamChiTiets { get; set; }
    }
}
