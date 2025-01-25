using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.PhongBenhViens
{
    public  class PhongBenhVienNhomGiaDichVuKhamBenh : BaseEntity
    {
        public long PhongBenhVienId { get; set; }
        public long NhomGiaDichVuKhamBenhBenhVienId { get; set; }
        public virtual PhongBenhVien PhongBenhVien { get; set; }
        public virtual NhomGiaDichVuKhamBenhBenhVien NhomGiaDichVuKhamBenhBenhVien { get; set; }
    }
}
