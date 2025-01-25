using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.DichVuKyThuats
{
    public class DichVuKyThuatBenhVienNoiThucHienUuTien : BaseEntity
    {
        public long DichVuKyThuatBenhVienId { get; set; }
        public long PhongBenhVienId { get; set; }
        public Enums.LoaiNoiThucHienUuTien LoaiNoiThucHienUuTien { get; set; }

        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien { get; set; }
        public virtual PhongBenhVien PhongBenhVien { get; set; }
    }
}
