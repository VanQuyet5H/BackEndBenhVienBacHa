using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.BaoCaoDanhSachBenhNhanPhauThuatValueObject
{
    public class BaoCaoDanhSachBenhNhanPhauThuatVo : GridItem
    {
        public DateTime? ThoiGianVaoVien { get; set; }

        public string ThoiGianVaoVienDisplay => ThoiGianVaoVien != null
            ? ThoiGianVaoVien.GetValueOrDefault().ApplyFormatDateTime()
            : string.Empty;

        public string MaTn { get; set; }

        public string HoTenBn { get; set; }

        public string DiaChi { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string NgaySinhDisplay =>
            NgaySinh != null ? NgaySinh.GetValueOrDefault().ApplyFormatDateTime() : string.Empty;

        public Enums.LoaiGioiTinh GioiTinh { get; set; }

        public string Khoa { get; set; }

        public string ChanDoan { get; set; }

        public string PhuongPhapPhauThuat { get; set; }

        public string PhuongPhapVoCam { get; set; }

        public string Ptv { get; set; }

        public string GayMe { get; set; }

        public string TinhTrangSauPt { get; set; }

        public DateTime? ThoiGianPhauThuatTu { get; set; }

        public DateTime? ThoiGianPhauThuatDen { get; set; }

        public string ThoiGianPhauThuatTuDisplay => ThoiGianPhauThuatTu != null
            ? ThoiGianPhauThuatTu.GetValueOrDefault().ApplyFormatDateTime()
            : string.Empty;

        public string ThoiGianPhauThuatDenDisplay => ThoiGianPhauThuatDen != null
            ? ThoiGianPhauThuatDen.GetValueOrDefault().ApplyFormatDateTime()
            : string.Empty;

        public string CapCuu { get; set; }

        public string TaiBien { get; set; }
    }
}
