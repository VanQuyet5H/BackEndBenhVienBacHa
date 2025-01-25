using Camino.Core.Domain.Entities.DichVuKyThuats;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.PhongBenhViens
{
    public class PhongBenhVienNhomGiaDichVuKyThuat : BaseEntity
    {
        public long PhongBenhVienId { get; set; }
        public long NhomGiaDichVuKyThuatBenhVienId { get; set; }
        public virtual PhongBenhVien PhongBenhVien { get; set; }
        public virtual NhomGiaDichVuKyThuatBenhVien NhomGiaDichVuKyThuatBenhVien { get; set; }
    }
}
