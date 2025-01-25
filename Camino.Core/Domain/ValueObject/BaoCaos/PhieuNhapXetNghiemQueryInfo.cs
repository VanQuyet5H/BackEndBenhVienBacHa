using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class PhieuXuatHoaChatQueryInfo : QueryInfo
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }

        public long KhoId { get; set; }
        public long DuocPhamId { get; set; }
        public long MayXetNghiemId { get; set; }
    }

    public class DanhSachPhieuXuatHoaChat : GridItem
    {
        public long KhoId { get; set; }
        public long DuocPhamId { get; set; }
        public long MayXetNghiemId { get; set; }

        public string TenKho { get; set; }
        public string MaDuocPham { get; set; }
        public string TenDuocPham { get; set; }
        public string DonViTinh { get; set; }
        public double? TongSoLuongXuat { get; set; }
    }


    public class DanhSachPhieuXuatHoaChatChiTiet : GridItem
    {
        public string TenDuocPham { get; set; }
        public DateTime NgayXuat { get; set; }
        public string NgayXuatDisplay => NgayXuat.ApplyFormatDate();
        public string SoLo { get; set; }
        public DateTime HanDung { get; set; }
        public string HanDungDisplay => HanDung.ApplyFormatDate();
        public double LuongXuat { get; set; }
        public string NguoiXuat { get; set; }
        public string NguoiNhan { get; set; }
    }   
}
