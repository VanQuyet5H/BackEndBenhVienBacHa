using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Camino.Api.Models.DinhMucDuocPhamTonKho
{
    public class DinhMucDuocPhamTonKhoViewModel : BaseViewModel
    {
        public long? DuocPhamBenhVienId { get; set; }
        public long? KhoDuocPhamId { get; set; }
        public string TenKhoDuocPham { get; set; }
        public string TenDuocPham { get; set; }
        public int? TonToiThieu { get; set; }
        public int? TonToiDa { get; set; }
        public int? SoNgayTruocKhiHetHan { get; set; }
        public string MoTa { get; set; }
    }
}
