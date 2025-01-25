using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;

namespace Camino.Core.Domain.Entities.GoiDichVus
{
    public class GoiDichVuChiTietDichVuGiuong : BaseEntity
    {
        public long GoiDichVuId { get; set; }

        public long DichVuGiuongBenhVienId { get; set; }

        public long NhomGiaDichVuGiuongBenhVienId { get; set; }

        public int SoLan { get; set; }

        public string GhiChu { get; set; }

        public virtual GoiDichVu GoiDichVu { get; set; }

        public virtual NhomGiaDichVuGiuongBenhVien NhomGiaDichVuGiuongBenhVien { get; set; }

        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
    }
}
