using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.PhongBenhViens;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class GoiKhamSucKhoeChungNoiThucHien : BaseEntity
    {
        public long? GoiKhamSucKhoeChungDichVuKhamBenhId { get; set; }
        public long? GoiKhamSucKhoeChungDichVuDichVuKyThuatId { get; set; }
        public long PhongBenhVienId { get; set; }

        public virtual GoiKhamSucKhoeChungDichVuKhamBenh GoiKhamSucKhoeChungDichVuKhamBenh { get; set; }
        public virtual GoiKhamSucKhoeChungDichVuDichVuKyThuat GoiKhamSucKhoeChungDichVuDichVuKyThuat { get; set; }
        public virtual PhongBenhVien PhongBenhVien { get; set; }
    }
}
