using System;
using System.Collections.Generic;

namespace Camino.Core.Domain.ValueObject.GiayCamKetKyThuatMois
{
    public class GiayCamKetKyThuatMoiVo
    {
        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public string QuanHe { get; set; }

        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string ChanDoan { get; set; }

        public string GiaiThich { get; set; }

        public double? SoTien { get; set; }

        public string SoTienChu { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string BsGmhs { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }
    }
    public class GiayThoaThuanLuaChonDichVuKhamTheoYeuCauVo
    {
        public GiayThoaThuanLuaChonDichVuKhamTheoYeuCauVo() {
            BacSiKhams = new List<long>();
        }
        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public string QuanHe { get; set; }

        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string KhiCanBaoTin { get; set; }

        public string BacSiKham { get; set; }
        public string NamTaiBuongLoai { get; set; }

        public double? SoTien { get; set; }

        public string SoTienChu { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string BsGmhs { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }
        public List<long> BacSiKhams { get; set; }
    }
    
    public class GiayCamKetKyThuatMoiHSVo
    {
        public int TaoLaAi { get; set; }

        public string HoTen { get; set; }

        public int? NamSinh { get; set; }

        public string QuanHe { get; set; }

        public int? GioiTinh { get; set; }

        public string DiaChi { get; set; }

        public string ChanDoan { get; set; }


        public double? SoTien { get; set; }

        public string SoTienChu { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string BacSyThucHien { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }

        public long? IdNoiTruHoSo { get; set; }
    }
}
