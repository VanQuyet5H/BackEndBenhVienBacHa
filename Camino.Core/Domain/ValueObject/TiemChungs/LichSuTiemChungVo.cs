using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.ValueObject.TiemChungs
{
    public class LichSuTiemChungGridVo : GridItem
    {
        public string MaYeuCauTiepNhan { get; set; }
        public string MaNguoiBenh { get; set; }
        public string HoTen { get; set; }
        public int? NamSinh { get; set; }
        public string DiaChi { get; set; }
        //public string MuiTiem { get; set; }
        public DateTime? ThoiDiemKham { get; set; }
        public string ThoiDiemKhamDisplay => ThoiDiemKham?.ApplyFormatDateTimeSACH();
        public long? BacSiKhamId { get; set; }
        public string BacSiKhamDisplay { get; set; }
        public LoaiPhanUngSauTiem? LoaiPhanUngSauTiem { get; set; }
        public string LoaiPhanUngSauTiemDisplay => LoaiPhanUngSauTiem.GetDescription();
        public DateTime? ThoiGianHenTiem { get; set; }
        public string ThoiGianHenTiemDisplay => ThoiGianHenTiem?.ApplyFormatDate();
    }

    public class LichSuTiemChungGridSearchVo
    {
        public long? YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public long? YeuCauDichVuKyThuatKhamSangLocId { get; set; }
        public string SearchString { get; set; }
        public int? ThoiGianCachLichHen { get; set; }
        public RangeDate ThoiDiemKham { get; set; }
    }

    public class RangeDate
    {
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
