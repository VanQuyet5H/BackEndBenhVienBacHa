using Camino.Core.Domain.Entities.BenhVien.Khoas;
using Camino.Core.Domain.Entities.KhoaPhongs;

namespace Camino.Core.Domain.Entities.KhoaPhongChuyenKhoas
{
    public class KhoaPhongChuyenKhoa : BaseEntity
    {
        public long KhoaPhongId { get; set; }

        public long KhoaId { get; set; }

        public virtual KhoaPhong KhoaPhong { get; set; }

        public virtual Khoa Khoa { get; set; }
    }
}
