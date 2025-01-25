using Camino.Core.Domain.ValueObject.Grid;
using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoVo
    {
        public HoSoKhacBienBanCamKetGayTeGiamDauTrongDeSauMoVo()
        {
            QuanHeThanNhans = new List<ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMo>();
        }

        public string BacSiGiaiThich { get; set; }
        public DateTime? NgayThucHien { get; set; }
        public long? BacSiGMHSId { get; set; }
        public string BacSiGMHSDisplay { get; set; }
        public List<ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMo> QuanHeThanNhans { get; set; }
    }

    public class ThongTinQuanHeThanNhanBienBanCamKetGayTeGiamDauTrongDeSauMo : GridItem
    {
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string CMND { get; set; }
        public long? QuanHeId { get; set; }
        public string QuanHeDisplay { get; set; }
        public string DiaChi { get; set; }
    }

    public class HoSoKhacBienBanInCamKetGayTeGiamDauTrongDeSauMo
    {
        public string Khoa { get; set; }
        public string NguoiBenh { get; set; }
        public string SinhNam { get; set; }
        public string CMNDCCCDHC { get; set; }
        public string NgayCap { get; set; }
        public string NoiCap { get; set; }
        public string MaSoBenhVien { get; set; }
        public string DiaChi { get; set; }
        public string QuanHeNhanThans { get; set; }
        public string BacSiGiaiThich { get; set; }
        public string BacSiGayMe { get; set; }
        public int Ngay { get; set; }
        public int Thang { get; set; }
        public int Nam { get; set; }
        public string BarCodeImgBase64 { get; set; }
        public string MaTN { get; set; }
        public string TenNB { get; set; }
    }
}