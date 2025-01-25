using Camino.Core.Domain.Entities.Thuocs;

namespace Camino.Core.Domain.Entities.HopDongThauDuocPhams
{
    public class HopDongThauDuocPhamChiTiet : BaseEntity
    {
        public long HopDongThauDuocPhamId { get; set; }

        public long DuocPhamId { get; set; }

        public decimal Gia { get; set; }

        public double SoLuong { get; set; }

        public double SoLuongDaCap { get; set; }

        public virtual HopDongThauDuocPham HopDongThauDuocPham { get; set; }

        public virtual DuocPham DuocPham { get; set; }
    }
}
