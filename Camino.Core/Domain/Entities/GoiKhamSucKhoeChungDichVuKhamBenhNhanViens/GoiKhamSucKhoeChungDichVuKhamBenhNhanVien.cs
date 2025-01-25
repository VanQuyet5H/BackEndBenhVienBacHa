using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.KhamDoans;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.GoiKhamSucKhoeChungDichVuKhamBenhNhanViens
{
    public class GoiKhamSucKhoeChungDichVuKhamBenhNhanVien :BaseEntity
    {
        public long GoiKhamSucKhoeId { get; set; }
        public long GoiKhamSucKhoeDichVuKhamBenhId { get; set; }
        public long DichVuKhamBenhBenhVienId { get; set; }
        public long HopDongKhamSucKhoeNhanVienId { get; set; }

        public virtual GoiKhamSucKhoeDichVuKhamBenh GoiKhamSucKhoeDichVuKhamBenh { get; set; }
        public virtual GoiKhamSucKhoe GoiKhamSucKhoe { get; set; }
        public virtual DichVuKhamBenhBenhVien DichVuKhamBenhBenhVien { get; set; }
        public virtual HopDongKhamSucKhoeNhanVien HopDongKhamSucKhoeNhanVien { get; set; }
    }
}
