using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.Thuocs;

namespace Camino.Core.Domain.Entities.KhoDuocPhams
{
    public class DuTruDuocPham : BaseEntity
    {
        public long KhoaPhongId { get; set; }
        public long NhanVienLapDuTruId { get; set; }
        public long DuocPhamId { get; set; }
        public int SoLuong { get; set; }
        public string Mota { get; set; }
        public bool? DuyetYeuCau { get; set; }

        public virtual KhoaPhong KhoaPhong { get; set; }
        public virtual NhanVien NhanVienLapDuTru { get; set; }
        public virtual DuocPham DuocPham { get; set; }
    }
}
