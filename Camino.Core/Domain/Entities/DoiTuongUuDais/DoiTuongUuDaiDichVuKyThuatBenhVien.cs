using Camino.Core.Domain.Entities.DichVuKyThuats;

namespace Camino.Core.Domain.Entities.DoiTuongUuDais
{
    public class DoiTuongUuDaiDichVuKyThuatBenhVien : BaseEntity
    {
        public long DoiTuongUuDaiId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public int TiLeUuDai { get; set; }

        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual DoiTuongUuDai DoiTuongUuDai { get; set; }
    }
}
