using Camino.Core.Domain.Entities.DuTruMuaDuocPhamKhoDuocs;
using Camino.Core.Domain.Entities.DuTruMuaDuocPhamTheoKhoas;
using Camino.Core.Domain.Entities.Thuocs;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.DuTruMuaDuocPhams
{
    public class DuTruMuaDuocPhamChiTiet : BaseEntity
    {
        public long DuTruMuaDuocPhamId { get; set; }
        public long DuocPhamId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public int SoLuongDuTru { get; set; }
        public int SoLuongDuKienSuDung { get; set; }
        public int? SoLuongDuTruTruongKhoaDuyet { get; set; }
        public string GhiChu { get; set; }
        public EnumNhomDieuTriDuPhong? NhomDieuTriDuPhong { get; set; }
        public long? DuTruMuaDuocPhamTheoKhoaChiTietId { get; set; }
        public long? DuTruMuaDuocPhamKhoDuocChiTietId { get; set; }
        public virtual DuocPham DuocPham { get; set; }
        public virtual DuTruMuaDuocPham DuTruMuaDuocPham { get; set; }
        public virtual DuTruMuaDuocPhamTheoKhoaChiTiet DuTruMuaDuocPhamTheoKhoaChiTiet { get; set; }
        public virtual DuTruMuaDuocPhamKhoDuocChiTiet DuTruMuaDuocPhamKhoDuocChiTiet { get; set; }
    }
}
