using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class PhieuNhapHoaChatQueryInfo : QueryInfo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public long KhoId { get; set; }
        public long DuocPhamId { get; set; }
    }

    public class DanhSachPhieuNhapHoaChat : GridItem
    {
        public long KhoId { get; set; }
        public long DuocPhamId { get; set; }
        public string TenKho { get; set; }
        public string MaDuocPham { get; set; }
        public string TenDuocPham { get; set; }
        public string DonViTinh { get; set; }
        public double? TongSoLuongNhap { get; set; }
    }


    public class DanhSachPhieuNhapHoaChatChiTiet : GridItem
    {
        public string TenDuocPham { get; set; }
        public DateTime NgayNhap { get; set; }
        public string NgayNhapDisplay => NgayNhap.ApplyFormatDate();
        public string SoLo { get; set; }
        public DateTime HanDung { get; set; }
        public string HanDungDisplay => HanDung.ApplyFormatDate();
        public double LuongNhap { get; set; }
        public string NguoiLinhTuKhoGoc { get; set; }
        public string NguoiNhap { get; set; }
        public string DanhSachMayXetNghiemId { get; set; }
    }
    public class LookupItemDuocPhamVo
    {
        public long KeyId { get; set; }

        public string DisplayName { get; set; }
    }
}
