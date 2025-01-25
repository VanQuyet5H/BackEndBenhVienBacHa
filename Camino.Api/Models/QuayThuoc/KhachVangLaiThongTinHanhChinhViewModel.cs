using Camino.Core.Domain;

namespace Camino.Api.Models.QuayThuoc
{
    public class KhachVangLaiThongTinHanhChinhViewModel
    {
        public long? BenhNhanId { get; set; }

        public string HoTen { get; set; }

        public Enums.LoaiGioiTinh? GioiTinh { get; set; }

        public int? NamSinh { get; set; }

        public string DiaChi { get; set; }

        public string SoDienThoai { get; set; }

        public long? TinhThanhId { get; set; }

        public long? QuanHuyenId { get; set; }

        public long? PhuongXaId { get; set; }
    }
}
