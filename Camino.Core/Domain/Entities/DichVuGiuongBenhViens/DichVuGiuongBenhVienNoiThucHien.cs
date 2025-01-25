using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.DichVuGiuongBenhViens
{
    public class DichVuGiuongBenhVienNoiThucHien: BaseEntity
    {
        public long DichVuGiuongBenhVienId { get; set; }
        public long? KhoaPhongId { get; set; }
        public long? PhongBenhVienId { get; set; }

        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
        public virtual KhoaPhong KhoaPhong { get; set; }
        public virtual PhongBenhVien PhongBenhVien { get; set; }
    }
}
