using System;

namespace Camino.Core.Domain.ValueObject.GiayKhamChuaBenhTheoYcs
{
    public class GiayKhamChuaBenhTheoYcVo
    {
        public string NguoiNhan { get; set; }

        public int TaoLaAi { get; set; }

        public string HoTenNguoiThan { get; set; }

        public int? NamSinhNguoiThan { get; set; }

        public int? GioiTinhNguoiThan { get; set; }

        public string CmndNguoiThan { get; set; }

        public string NoiCapCmndNguoiThan { get; set; }

        public long? DanTocNguoiThan { get; set; }

        public long? NgoaiKieuNguoiThan { get; set; }

        public long? NgheNghiepNguoiThan { get; set; }

        public string NoiLamViecNguoiThan { get; set; }

        public string DiaChiNguoiThan { get; set; }

        public string KhiCanBaoTin { get; set; }

        public string BacSiChamSoc { get; set; }

        public string LoaiChuaTri { get; set; }

        public double? SoTien { get; set; }

        public string SoTienChu { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string BsDieuTri { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }
    }
}
