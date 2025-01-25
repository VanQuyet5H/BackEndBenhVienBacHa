using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.LoaiGiaDichVus
{
    public class LoaiGiaDichVuGridVo: GridItem
    {
        public long Id { get; set; }
        public string Ten { get; set; }
        public Enums.NhomDichVuLoaiGia Nhom { get; set; }
        public string TenNhom => Nhom.GetDescription();
    }
}
