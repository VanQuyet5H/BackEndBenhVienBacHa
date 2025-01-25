using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject
{
    public class KetQuaCDHATDCNTimKiemGridVo : GridItem
    {
        public long YeuCauDichVuKyThuatId { get; set; }
        public string MaYeuCauTiepNhan { get; set; }
        public string SoBenhAn { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }

        public string GioiTinh { get; set; }
        public string ChuanDoan { get; set; }
        public string ChuanDoanDisplay { get; set; }
        public string BacSiCD { get; set; }
        public string BacSiCDDisplay { get; set; }

        public string DataKetQuaCanLamSang { get; set; }
        public string KetLuanDisplay { get; set; }

        public string BacSiKetLuan { get; set; }
        public string BacSiKetLuanDisplay { get; set; }
        public string KyThuatVien1 { get; set; }
        public string KyThuatVien1Display { get; set; }

        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgayThangNam => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);

        public DateTime NgayChiDinh { get; set; }

        public string NgayChiDinhDisplay { get; set; }
        public string NoiChiDinh { get; set; }
        public string ChanDoan { get; set; }
        public string ChiDinh { get; set; }
        public string DoiTuong { get; set; }
        public bool TrangThai { get; set; }
        public string TenTrangThai => TrangThai ? "Đã có kết quả" : "Chờ kết quả";

        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienDisplay { get; set; }

    }
}






