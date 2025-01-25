using Camino.Core.Domain.Entities.DichVuKyThuats;
using Camino.Core.Domain.Entities.KhamDoans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKyThuatNhanViens
{
    public class GoiKhamSucKhoeChungDichVuKyThuatNhanVien : BaseEntity
    {
        public long GoiKhamSucKhoeId { get; set; }
        public long GoiKhamSucKhoeDichVuDichVuKyThuatId { get; set; }
        public long DichVuKyThuatBenhVienId { get; set; }
        public long HopDongKhamSucKhoeNhanVienId { get; set; }

        public virtual GoiKhamSucKhoeDichVuDichVuKyThuat GoiKhamSucKhoeDichVuDichVuKyThuat { get; set; }
        public virtual GoiKhamSucKhoe GoiKhamSucKhoe { get; set; }
        public virtual DichVuKyThuatBenhVien DichVuKyThuatBenhVien{ get; set; }
        public virtual HopDongKhamSucKhoeNhanVien HopDongKhamSucKhoeNhanVien { get; set; }
    }
}
