using System;
using System.Collections.Generic;
using System.ComponentModel;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaos
{
    public class ThongKeBacSiKeDonTheoThuocQueryInfo : QueryInfo
    {
        public long BacSiId { get; set; }
        public long ThuocId { get; set; }

        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
    }
    public class ThongKeBacSiKeDonTheoThuocDataVo
    {
        public long BacSiKeDonId { get; set; }
        public long KhoaPhongId { get; set; }
        public string TenBacSi { get; set; }
        public string KhoaPhongBacSi { get; set; }
        public double SoLuongDon { get; set; }
        public double SoLuongDaKe { get; set; }

    }
    public class DanhSachThongKeBacSiKeDonTheoThuoc : GridItem
    {     
        public string TenBacSi { get; set; }
        public string KhoaPhongBacSi { get; set; }
        public double SoLuongDon { get; set; }
        public double SoLuongDaKe { get; set; }

    }

    public class DuocPhamBenhVienLookupItemVo : LookupItemVo
    {
        public string HamLuong { get; set; }
    }

}
