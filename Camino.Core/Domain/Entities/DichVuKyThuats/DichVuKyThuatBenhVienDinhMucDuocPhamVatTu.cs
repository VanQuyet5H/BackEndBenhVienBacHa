using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.VatTuBenhViens;

namespace Camino.Core.Domain.Entities.DichVuKyThuats
{
    public class DichVuKyThuatBenhVienDinhMucDuocPhamVatTu : BaseEntity
    {
        public long DichVuKyThuatBenhVienId { get; set; }
        public long? DuocPhamBenhVienId { get; set; }
        public long? VatTuBenhVienId { get; set; }
        public double SoLuong { get; set; }
        public bool? KhongTinhPhi { get; set; }

        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
    }
}