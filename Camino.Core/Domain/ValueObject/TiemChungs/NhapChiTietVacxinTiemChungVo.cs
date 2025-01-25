using Camino.Core.Domain.Entities.NhapKhoDuocPhamChiTiets;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class NhapChiTietVacxinTiemChungVo
    {
        public long NhapKhoDuocPhamChiTietId { get; set; }
        public NhapKhoDuocPhamChiTiet NhapKhoDuocPhamChiTiet { get; set; }
        public double SoLuongXuat { get; set; }
    }
}
