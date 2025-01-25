using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.QuayThuoc
{
    public class ThongTinDuocPhamViewModel : BaseViewModel
    {
        public long DuocPhamId { get; set; }
        public long HoatChatId { get; set; }
        public long TenDuocPham { get; set; }
        public long SoLuongTon { get; set; }
        public long NhapKhoDuocPhamChiTietId { get; set; }
        public long TenHoatChat { get; set; }
        public string DonViTinh { get; set; }
        public int SoLuongToa { get; set; }
        public int SoLuongMua { get; set; }
        public int DonGia { get; set; }
        public string Solo { get; set; }
        public string ViTri { get; set; }
        public string HanSuDung { get; set; }
        public string BacSiKeToa { get; set; }
        public long BacSiId { get; set; }
    }
}
