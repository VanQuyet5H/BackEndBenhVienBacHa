using Camino.Core.Domain.Entities.DichVuKyThuats;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.GoiDichVus
{
    public class GoiDichVuChiTietDichVuKyThuat : BaseEntity
    {
        public long GoiDichVuId { get; set; }

        public long DichVuKyThuatBenhVienId { get; set; }

        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }

        public int SoLan { get; set; }

        public string GhiChu { get; set; }

        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }

        public virtual NhomGiaDichVuKyThuatBenhVien NhomGiaDichVuKyThuatBenhVien { get; set; }

        public virtual GoiDichVu GoiDichVu { get; set; }

    }
}
