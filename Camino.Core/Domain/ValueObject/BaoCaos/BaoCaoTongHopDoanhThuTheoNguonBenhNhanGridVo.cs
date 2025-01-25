using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class BaoCaoTongHopDoanhThuTheoNguonBenhNhanChiTietGridVo : GridItem
    {
        public string NguonBenhNhan { get; set; }
        public long? NoiGioiThieuId { get; set; }
        public long? HinhThucDenId { get; set; }
        public long? BenhNhanId { get; set; }
        public long? HopDongKhamSucKhoeNhanVienId { get; set; }
        public long? CongTyKhamSucKhoeId { get; set; }

        public DateTime NgayThu { get; set; }

        public decimal? GiaNhapKho { get; set; }
        public decimal? DoanhThuThucTe { get; set; }
        public decimal? GiaNiemYet { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public decimal? BHYTChiTra { get; set; }
        public decimal HeSo { get; set; }
    }
    public class BaoCaoTongHopDoanhThuTheoNguonBenhNhanGridVo:GridItem
    {
        public string NguonKhachHang { get; set; }
        public int SoLuongKhachHang { get; set; }
        public decimal? DoanhThuTheoGiaNiemYet { get; set; }
        public decimal? MienGiam { get; set; }
        public decimal? BaoHiemChiTra { get; set; }
        public decimal? DoanhThuThucTeDuocThuTuKhachHang { get; set; }
    }

    public class BaoCaoTongHopDoanhThuTheoNguonBenhNhanQueryInfo :QueryInfo
    {
        public string TimKiemTheoTuKhoa { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }
    public class SumTongHopDoanhThuTheoNguonBenhNhanGridVo
    {
        public int TotalSoLuongKhachHang { get; set; }
        public decimal? TotalDoanhThuTheoGiaNiemYet { get; set; }
        public decimal? TotalMienGiam { get; set; }
        public decimal? TotalBaoHiemChiTra { get; set; }
        public decimal? TotalDoanhThuThucTeDuocThuTuKhachHang { get; set; }
    }
}
