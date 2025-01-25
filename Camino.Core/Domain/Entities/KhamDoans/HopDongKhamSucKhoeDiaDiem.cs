using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.KhamDoans
{
    public class HopDongKhamSucKhoeDiaDiem : BaseEntity
    {
        public long HopDongKhamSucKhoeId { get; set; }
        public string DiaDiem { get; set; }
        public string CongViec { get; set; }
        public DateTime? Ngay { get; set; }
        public string GhiChu { get; set; }

        public virtual HopDongKhamSucKhoe HopDongKhamSucKhoe { get; set; }
    }
}
