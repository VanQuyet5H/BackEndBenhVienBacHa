using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.LoaiGiaDichVus
{
    public class LoaiGiaDichVuInfoVo
    {
        public long Id { get; set; }
        public Enums.NhomDichVuLoaiGia Nhom { get; set; }
    }
}
