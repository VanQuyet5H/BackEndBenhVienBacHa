using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class KiemTraNguoiBenhTrongHeThongVo
    {
        public bool? CoBHYT { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgayThangNamSinh { get; set; }
        public int? NgaySinh => NgayThangNamSinh?.Day;
        public int? ThangSinh => NgayThangNamSinh?.Month;
        public int? NamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Message { get; set; }
    }
}
