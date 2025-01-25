using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.KhamDoan
{
    public class KhamDoanThongTinHanhChinhVo : GridItem
    {
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string TenGioiTinh { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public string TenNgheNghiep { get; set; }
        public string TenDanToc { get; set; }
        public string DiaChiDisplay { get; set; }
        public string TenCongTy { get; set; }
    }
}
