using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.PhieuTheoDoiTruyenDich
{
    public class PhieuTheoDoiTruyenDichGridVo : GridItem
    {
        public DateTime NgayThu { get;set; }
        public long IdTruyenDich { get; set; }
        public string TenTruyenDich { get; set; }
        public double SoLuong { get; set; }
        public string LoSoSX { get; set; }
        public int? TocDo { get; set; }
        public int? BatDau { get; set; }

        public int? KetThuc { get; set; } 
        public string BSChiDinh { get; set; }
        public string YTaThucHien { get; set; }
        public bool FistCheck { get; set; }
        public string Ngay { get; set; }
        // update  4/62021
        public bool NoiTruChiTietYLenhThucHien { get; set; }
        public long? NoiTruPhieuDieuTriChiTietYLenhId { get; set; }
        public long? YeuCauLinhDuocPhamId { get; set; }

    }
    public class PhieuTheoDoiTruyenDichGrid
    {
        public string TenNhanVien { get; set; }
        public string NgayThucHien { get; set; }
        public string ChanDoan { get; set; }
        public List<PhieuTheoDoiTruyenDichGridVo> ListPhieuTheoDoiTruyenDich { get; set; }
        public List<DuocPhamPhieuDieuTriTheoNgayDefault> ListPhieuTheoDoiTruyenDichDefault { get; set; }
    }

    public class ObjDanhSachThuocTheoNgay
    {
        public long YeuCauTiepNhanId { get; set; }
        public List<TenThuoc> ListDanhSach { get; set; }
    }
    public class TenThuoc
    {
        public long? IdTruyenDich { get; set; }
        public double? SL { get; set; }
        public DateTime? NgayThu { get; set; }
        public string TenDuocPham { get; set; }
    }
    public class DuocPhamPhieuDieuTriTheoNgay
    {
        public long IdTruyenDich { get; set; }
        public DateTime NgayPhieuDieuTri { get; set; }
        public string TenDuocPhamTruyenDich { get; set; }
        public double Sl { get; set; }
        public string LoSoSX { get; set; }
        public int? TocDo { get; set; }
        public int? BatDau { get; set; }
        public long NoiTruChiDinhDuocPhamId { get; set; }

        public int? KetThuc { get; set; }
        public string BSChiDinh { get; set; }
        public string YTaThucHien { get; set; }
        public bool NoiTruChiTietYLenhThucHien { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
    }
    public class DuocPhamPhieuDieuTriTheoNgayDefault
    {
        public int TotalSlTheoDuocPham { get; set; }
        public string TenDuocPhamTruyenDich { get; set; }
        public DateTime NgayThu { get; set; }

    }
}
