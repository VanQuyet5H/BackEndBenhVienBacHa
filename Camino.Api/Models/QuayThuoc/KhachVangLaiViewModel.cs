using System;

namespace Camino.Api.Models.QuayThuoc
{
    public class KhachVangLaiViewModel
    {
        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public int[] HinhThucThanhToan { get; set; }

        public decimal? TienMat { get; set; }

        public decimal? ChuyenKhoan { get; set; }

        public decimal? POS { get; set; }

        public decimal? SoTienCongNo { get; set; }

        public decimal? BenhNhanDua { get; set; }

        public DateTime NgayThu { get; set; }

        public string NoiDungThu { get; set; }

        public decimal? TongTien { get; set; }
    }
}
