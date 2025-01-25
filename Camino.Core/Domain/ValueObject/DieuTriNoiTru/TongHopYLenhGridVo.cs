using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class TongHopYLenhTimKiemNangCaoVo
    {
        public string SearchString { get; set; }
        public long? KhoaId { get; set; }
        public long? PhongId { get; set; }
        public bool ChuaHoanThanh { get; set; }
        public DateTime? NgayYLenh { get; set; }
        public string strNgayYLenh { get; set; }
    }

    public class TongHopYLenhGridVo : GridItem
    {
        public string Giuong { get; set; }
        public string Phong { get; set; }
        public string MaBenhAn { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }
        public string NgaySinhDisplay => NgaySinh != null && ThangSinh != null && NgaySinh != 0 && ThangSinh != 0 ? NgaySinh + "/" + ThangSinh + "/" + NamSinh : (NamSinh == null ? "" : NamSinh.ToString());
        public string GioiTinh { get; set; }
        public string BacSi { get; set; }
        public string YTa { get; set; }
        public int SoLuongYLenh { get; set; }
        public int SoLuongYLenhHoanThanh { get; set; }
        public DateTime NgayYLenh { get; set; }
        public string SoLuong => SoLuongYLenh == 0 && SoLuongYLenhHoanThanh == 0 ? null : SoLuongYLenhHoanThanh + "/" + SoLuongYLenh;
    }

    public class ThongTinBenhAnCoChiDinhKhamVo
    {
        public long NoiTruBenhAnId { get; set; }
        public long YeuCauTiepNhanNgoaiTruId { get; set; }
        public DateTime? ThoiDiemTongHopYLenhKhamBenh { get; set; }
    }
}
