using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class HoSoKhacGiayTuNguyenTrietSanVo
    {
        public DateTime NgayThucHien { get; set; }
        public string NgayThucHienDisplay => NgayThucHien.ApplyFormatDateTimeSACH();
        public long? BacSiThucHienId { get; set; }
        public string BacSiThucHienDisplay { get; set; }
    }

    public class HoSoKhacGiayInTuNguyenTrietSan
    {
        public string Khoa { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string HoTen { get; set; }
        public string Tuoi { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string NguoiTrietSan { get; set; }
        public string NguoiThanTrietSan { get; set; }
        public string NhanVienTrietSan { get; set; }
        public string Ngay { get; set; }
        public string Thang { get; set; }
        public string Nam { get; set; }
        public string MaNB { get; set; }
    }
}