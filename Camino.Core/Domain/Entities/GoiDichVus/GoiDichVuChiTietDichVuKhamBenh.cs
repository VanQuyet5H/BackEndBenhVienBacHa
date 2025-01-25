using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.GoiDichVus
{
    public class GoiDichVuChiTietDichVuKhamBenh : BaseEntity
    {
        public long GoiDichVuId { get; set; }

        public long DichVuKhamBenhBenhVienId { get; set; }

        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }

        public int SoLan { get; set; }

        public string GhiChu { get; set; }

        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }

        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }

        public virtual GoiDichVu GoiDichVu { get; set; }

      
    }
}
