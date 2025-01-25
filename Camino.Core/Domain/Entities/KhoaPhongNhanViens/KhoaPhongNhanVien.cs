using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.KhoaPhongNhanViens
{
    public class KhoaPhongNhanVien : BaseEntity
    {
        public long KhoaPhongId { get; set; }

        public long NhanVienId { get; set; }

        public long? PhongBenhVienId { get; set; }

        public bool? LaPhongLamViecChinh { get; set; }

        public virtual PhongBenhVien PhongBenhVien { get; set; }

        public virtual KhoaPhong KhoaPhong { get; set; }

        public virtual NhanVien NhanVien { get; set; }
    }
}
