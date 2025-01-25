using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.DichVukyThuatBenhVienMauKetQua
{
    public class DichVukyThuatBenhVienMauKetQua : BaseEntity
    {
        public string TenKetQuaMau { get; set; }
        public string MaSo { get; set; }
        public string KetQua { get; set; }
        public string KetLuan { get; set; }
        public long NhanVienThucHienId { get; set; }
        public virtual NhanVien NhanVienThucHien { get; set; }

        public virtual Camino.Core.Domain.Entities.DichVuKyThuats.DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
    }
}
