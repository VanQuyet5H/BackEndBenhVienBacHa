using Camino.Core.Helpers;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Camino.Api.Models.TiemChung
{
    public class TiemChungKetQuaSinhHieuViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long YeuCauDichVuKyThuatKhamSangLocTiemChungId { get; set; }
        public int? NhipTim { get; set; }
        public int? HuyetApTamThu { get; set; }
        public int? HuyetApTamTruong { get; set; }
        public string HuyetAp => $"{HuyetApTamThu}/{HuyetApTamTruong}";
        public int? NhipTho { get; set; }
        public double? ThanNhiet { get; set; }
        public double? CanNang { get; set; }
        public double? ChieuCao { get; set; }
        public double? BMI { get; set; }
        public double? Glassgow { get; set; }
        public double? SpO2 { get; set; }
        public long? NoiThucHienId { get; set; }
        public long NhanVienThucHienId { get; set; }
        public DateTime ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien.ApplyFormatDateTimeSACH();

        [NotMapped]
        public bool IsEditable { get; set; }
    }
}