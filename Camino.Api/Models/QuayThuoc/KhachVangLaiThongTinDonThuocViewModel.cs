using System;

namespace Camino.Api.Models.QuayThuoc
{
    public class KhachVangLaiThongTinDonThuocViewModel
    {
        public string HoTenBenhNhan { get; set; }

        public decimal? TienMat { get; set; }

        public decimal? BenhNhanDua { get; set; }

        public int?[] HinhThucThanhToan { get; set; }

        public decimal? TienTraLai { get; set; }

        public decimal? ChuyenKhoan { get; set; }

        public decimal? POS { get; set; }

        public decimal? SoTienCongNo { get; set; }

        public DateTime NgayThu { get; set; }

        public string NoiDungThu { get; set; }
        public string GhiChu { get; set; }
    }
}
