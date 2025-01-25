using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject
{
    public class KetQuaCDHATDCNLichSuGridVo : GridItem
    {
        public string MaYeuCauTiepNhan { get; set; }
        public string SoBenhAn { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
    }


    public class ChiTietKetQuaCDHATDCNLichSuGridVo : GridItem
    {
        public string DichVu { get; set; }
        public string NguoiChiDinh { get; set; }
        public string NoiChiDinh { get; set; }
        public DateTime? NgayChiDinh { get; set; }
        public string NgayChiDinhDisplay => NgayChiDinh?.ApplyFormatDate();
        public string NguoiThucHien { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public string NgayThucHienDisplay => NgayThucHien?.ApplyFormatDate();
        public string BacSiKetLuan { get; set; }
        public string MayTraKetQua { get; set; }
        public string FileChuKy { get; set; }
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
        public string HtmlContentKetQua { get; set; }
    }
}
