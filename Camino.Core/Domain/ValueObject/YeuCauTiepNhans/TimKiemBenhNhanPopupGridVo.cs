using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class TimKiemBenhNhanPopupGridVo : GridItem
    {
        public string MaBN { get; set; }
        public string BHYTMaSoThe { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => DateHelper.DOBFormat(NgaySinh, ThangSinh, NamSinh);
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhDisplay => GioiTinh.GetDescription();
        public string SoChungMinhThu { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string SoDienThoaiDisplay { get; set; }
        public string NgaySinhFormat { get; set; }
    }
}
