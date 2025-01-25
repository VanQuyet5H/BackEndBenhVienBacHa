using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens
{
    public class DichVuKhamBenhBenhVienNoiThucHien : BaseEntity
    {
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long? KhoaPhongId { get; set; }
        public long? PhongBenhVienId { get; set; }

        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        public virtual KhoaPhong KhoaPhong { get; set; }
        public virtual PhongBenhVien PhongBenhVien { get; set; }
    }
}
