using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.XuatKhos
{
    public class DuocPhamCanXuatVo
    {
        public long DuocPhamBenhVienId { get; set; }
        public bool LaDuocPhamBHYT { get; set; }
        public double SoLuongXuat { get; set; }
    }
}
