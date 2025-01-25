namespace Camino.Core.Domain.Entities.PhauThuatThuThuats
{
    public class KhamTheoDoiBoPhanKhac : BaseEntity
    {
        public string Ten { get; set; }
        public string NoiDung { get; set; }
        public long KhamTheoDoiId { get; set; }

        public virtual KhamTheoDoi KhamTheoDoi { get; set; }
    }
}
