using Camino.Core.Domain;
using System;

namespace Camino.Api.Models.QuayThuoc
{
    public class KhachVangLaiThuocChoThanhToanViewModel
    {
        public int? STT { get; set; }

        public long? DuocPhamId { get; set; }

        public string MaHoatChat { get; set; }

        public string TenDuocPham { get; set; }

        public double? SoLuongTon { get; set; }

        public long? NhapKhoDuocPhamChiTietId { get; set; }

        public string TenHoatChat { get; set; }

        public string DonViTinh { get; set; }

        public double? SoLuongToa { get; set; }

        public double? SoLuongMua { get; set; }

        public decimal? DonGia { get; set; }

        public decimal? ThanhTien { get; set; }

        public string Solo { get; set; }

        public string ViTri { get; set; }

        public DateTime HanSuDung { get; set; }

        public string HanSuDungHientThi { get; set; }

        public string BacSiKeToa { get; set; }
        public Enums.LoaiDuocPhamHoacVatTu LoaiDuocPhamHoacVatTu { get; set; }
    }
}
