using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class GoiKhamSucKhoeNoiThucHien : BaseEntity
    {
        public long? GoiKhamSucKhoeDichVuKhamBenhId { get; set; }
        public long? GoiKhamSucKhoeDichVuDichVuKyThuatId { get; set; }
        public long PhongBenhVienId { get; set; }

        public virtual GoiKhamSucKhoeDichVuKhamBenh GoiKhamSucKhoeDichVuKhamBenh { get; set; }
        public virtual GoiKhamSucKhoeDichVuDichVuKyThuat GoiKhamSucKhoeDichVuDichVuKyThuat { get; set; }
        public virtual PhongBenhVien PhongBenhVien { get; set; }
    }
}
