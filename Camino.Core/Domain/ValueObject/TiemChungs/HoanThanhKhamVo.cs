using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class HoanThanhKhamTiemChungGridVo : GridItem
    {
        public string MaYeuCauTiepNhan { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public int? NgaySinh { get; set; }
        public int? ThangSinh { get; set; }
        public int? NamSinh { get; set; }

        public string NamSinhDisplay => (NgaySinh != null && ThangSinh != null && NamSinh != null && NgaySinh != 0 && ThangSinh != 0 && NamSinh != 0)
            ? (new DateTime(NamSinh.Value, ThangSinh.Value, NgaySinh.Value)).ApplyFormatDate()
            : NamSinh?.ToString(); 
        public string DiaChiDayDu { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemKhamDisplay => ThoiDiemThucHien?.ApplyFormatDateTimeSACH();
        public string BacSiKhamDisplay { get; set; }
        public string MuiTiem { get; set; }
        public string NoiTheoDoiSauTiem { get; set; }
        public long PhongBenhVienId { get; set; }
        public Enums.EnumTrangThaiYeuCauDichVuKyThuat? TrangThaiMuiTiemVacxin { get; set; }

        //cập nhật 03/03/2022
        public long? KhamSangLocTiemChungId { get; set; }
    }

    public class HoanThanhKhamTiemChungTimKiemNangCaoVo
    {
        public string SearchString { get; set; }
        public long? VacxinId { get; set; }
        public Enums.LoaiHangDoiTiemVacxin LoaiHangDoi { get; set; }
        public TuNgayDenNgayVo TuNgayDenNgay { get; set; }
    }

    public class TuNgayDenNgayVo
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string TuNgay { get; set; }
        public string DenNgay { get; set; }
    }

    public class KhamTiemChungMoLaiVo
    {
        public long YeuCauKhamTiemChungId { get; set; }
        public long PhongBenhVienId { get; set; }
    }
}
