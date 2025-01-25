using System.Collections.Generic;

namespace Camino.Api.Models.QuayThuoc
{
    public class KhachVangLaiChoThanhToanViewModel
    {
        public KhachVangLaiChoThanhToanViewModel()
        {
            DanhSachDonThuoc = new List<KhachVangLaiThuocChoThanhToanViewModel>();
        }

        public KhachVangLaiThongTinHanhChinhViewModel ThongTinKhach { get; set; }

        public KhachVangLaiThongTinDonThuocViewModel ThongTinThuChi { get; set; }

        public List<KhachVangLaiThuocChoThanhToanViewModel> DanhSachDonThuoc { get; set; }
    }
}
