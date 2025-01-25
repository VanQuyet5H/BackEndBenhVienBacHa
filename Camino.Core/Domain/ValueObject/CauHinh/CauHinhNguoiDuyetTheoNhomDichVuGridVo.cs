using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.CauHinh
{
    public class CauHinhNguoiDuyetTheoNhomDVGridVo :GridItem
    {
        public long? NhanVienDuyetId { get; set; }
        public string TenNhanVienDuyet { get; set; }
        public long? NhomDichVuBenhVienId { get; set; }
        public string TenNhomDichVuBenhVien { get; set; }
    }
}
