using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.DuTruMuaDuocPhamTaiKhoaDuoc
{
    public class DuTruMuaDuocPhamChiTietGridVo : GridItem
    {
        public long DuTruMuaDuocPhamId { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int? SoLuongDuTruTruongKhoaDuyet { get; set; }
        public EnumNhomDieuTriDuPhong? NhomDieuTriDuPhong { get; set; }
        public long? DuTruMuaDuocPhamTheoKhoaChiTietId { get; set; }
        public long? DuTruMuaDuocPhamKhoDuocChiTietId { get; set; }
    }
    public class DuTruMuaDuocPhamKhoaChiTietGridVo : GridItem
    {
        public long DuTruMuaDuocPhamTheoKhoaId { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int SoLuongDuTruTruongKhoaDuyet { get; set; }
        public int? SoLuongDuTruKhoDuocDuyet { get; set; }
        public long? DuTruMuaDuocPhamKhoDuocChiTietId { get; set; }
    }
}
