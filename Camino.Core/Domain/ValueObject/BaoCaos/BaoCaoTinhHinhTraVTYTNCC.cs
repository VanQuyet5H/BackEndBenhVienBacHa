using System;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoTinhHinhTraVTYTNCCQueryInfo : QueryInfo
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long KhoId { get; set; }
    }

    public class BaoCaoTinhHinhTraVTYTNCCGridVo : GridItem
    {
        public DateTime? NgayHoaDon { get; set; }
        public string NgayHoaDonStr => NgayHoaDon.HasValue ? NgayHoaDon.Value.ToString("dd/MM/yy") : "";
        public DateTime? NgayTra { get; set; }
        public string NgayTraStr => NgayTra.HasValue ? NgayTra.Value.ToString("dd/MM/yy") : "";
        public string SoHoaDon { get; set; }
        public string SoPhieuTra { get; set; }
        public string CongTy { get; set; }
        public string MaVTYT { get; set; }
        public string TenVTYT { get; set; }
        public string DVT { get; set; }
        public string SoLo { get; set; }
        public double? SoLuongTra { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public decimal? ThanhTien => (decimal)(SoLuongTra ?? 0) * (DonGiaNhap ?? 0);
        public string DienGiai => !string.IsNullOrEmpty(CongTy) ? $"Trả lại công ty: {CongTy}" : "";
        public string Nhom { get; set; }
    }
}
