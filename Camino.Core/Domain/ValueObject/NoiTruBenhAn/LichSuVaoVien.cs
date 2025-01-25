using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.NoiTruBenhAn
{
    public class LichSuVaoVienGridVo : GridItem
    {
        public string MaTiepNhan { get; set; }
        public string SoBenhAnh { get; set; }
        public string MaBenhNhan { get; set; }
        public DateTime ThoiGianTiepNhan { get; set; }
        public string ThoiGianTiepNhanDisplay { get; set; }

        public long BenhNhanId { get; set; }
        public string ChuanDoan { get; set; }
        public string BacSi { get; set; }
        public string PhongKham { get; set; }
        public bool KiemTraNoiTru { get; set; }
    }
    public class ThongTinTheoKhamBenh
    {
        public string BacSi { get; set; }
        public string PhongKham { get; set; }
        public string NgayKham { get; set; }
    }

    public class ThongTinLichSuKhamBenhNoiTru
    {
        public string LyDoVaoVien { get; set; }
        public string BenhSu { get; set; }
        public string KhamToanThan { get; set; }
        public string ICD { get; set; }
        public string ChuanDoan { get; set; }

        public List<CoQuanBoPhan> KhamCaCoQuans { get; set; }
        public List<CoQuanBoPhan> KhamKhacs { get; set; }

    }
    public class CoQuanBoPhan
    {
        public string TenBoPhan { get; set; }
        public string LyDo { get; set; }
    }
}
