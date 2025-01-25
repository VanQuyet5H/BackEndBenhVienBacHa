using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.KhamBenhs
{
    public class ChiDinhDichVuCuaBacSiKhacVo : GridItem
    {
        public string TenNhom { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public string TenLoaiGia { get; set; }
        public double SoLuong { get; set; }
        public string TenNguoiChiDinh { get; set; }
        public Enums.EnumNhomGoiDichVu NhomId { get; set; }
        public string Nhom { get; set; }
        public bool? CheckRowItem { get; set; }
    }
}
