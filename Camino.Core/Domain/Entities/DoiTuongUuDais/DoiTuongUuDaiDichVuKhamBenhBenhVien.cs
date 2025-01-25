using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;

namespace Camino.Core.Domain.Entities.DoiTuongUuDais
{
    public class DoiTuongUuDaiDichVuKhamBenhBenhVien : BaseEntity
    {
        public long DoiTuongUuDaiId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public int TiLeUuDai { get; set; }

        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        public virtual DoiTuongUuDai DoiTuongUuDai { get; set; }
    }
}
