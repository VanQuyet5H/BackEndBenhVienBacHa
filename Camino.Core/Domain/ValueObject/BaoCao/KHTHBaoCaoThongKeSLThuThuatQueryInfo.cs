using Camino.Core.Domain.ValueObject.Grid;
using System;

namespace Camino.Core.Domain.ValueObject.BaoCao
{
    public class KHTHBaoCaoThongKeSLThuThuatQueryInfo : QueryInfo
    {
        public long KhoaId { get; set; }
        public long PhongBenhVienId { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }

    public class KHTHBaoCaoThongKeSLThuThuat : GridItem
    {
        public bool ToDam { get; set; }
        public bool Center { get; set; }
        public bool GachChan { get; set; }

        public string TenKhoaPhong { get; set; }
        public string DanhMucThuThuat { get; set; }
        public string ThongTuKhoa { get; set; }
        public string ThongTuSo { get; set; }

        public int? TongSoLuongThuThuatDacBiet { get; set; }
        public int? TongSoLuongThuThuat1 { get; set; }
        public int? TongSoLuongThuThuat2 { get; set; }
        public int? TongSoLuongThuThuat3 { get; set; }

        public int? Phien { get; set; }
        public int? CapCuu { get; set; }

        public int? TongSo => (TongSoLuongThuThuatDacBiet ?? 0) + (TongSoLuongThuThuat1 ?? 0) + (TongSoLuongThuThuat2 ?? 0) + (TongSoLuongThuThuat3 ?? 0);
    }

    public class KHTHBaoCaoThongKeSLThuThuatChiTiet
    {
        public long Id { get; set; }
        public long KhoaPhongId { get; set; }
        public string TenKhoaPhong { get; set; }
        public long PhongBenhVienId { get; set; }
        public string TenPhongBenhVien { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public string TenDichVuThuThuat { get; set; }
        public string ThongTuKhoa { get; set; }
        public string ThongTuSo { get; set; }
        public string LoaiThuThuat { get; set; }
        public int SoLan { get; set; }
    }

    public class KhoaDaChon
    {
        public long KhoaId { get; set; }
    }
}
