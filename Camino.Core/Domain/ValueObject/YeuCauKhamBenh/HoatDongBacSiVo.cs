using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauKhamBenh
{
    public class HoatDongBacSiVo
    {
        public long PhongBenhVienId { get; set; }
        public string HoTen { get; set; }
        public long NhanVienId { get; set; }
        public DateTime ThoiDiemBatDau { get; set; }
    }
}
