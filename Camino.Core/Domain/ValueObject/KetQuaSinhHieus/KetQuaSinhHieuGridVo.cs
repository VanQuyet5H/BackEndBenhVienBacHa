using System;
using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;

namespace Camino.Core.Domain.ValueObject.KetQuaSinhHieus
{
    public class KetQuaSinhHieuGridVo : GridItem
    {
        public int? NhipTim { get; set; }

        public int? NhipTho { get; set; }

        public double? ThanNhiet { get; set; }

        public string HuyetAp { get; set; }

        public int? HuyetApTamThu { get; set; }

        public int? HuyetApTamTruong { get; set; }

        public double? ChieuCao { get; set; }

        public double? CanNang { get; set; }

        public double? BMI { get; set; }

        public double? SpO2 { get; set; }

        public string NgayThucHien { get; set; }

        public double? Glassgow { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsDelete { get; set; }

        public bool IsSave { get; set; }

        public int? KSKPhanLoaiTheLuc { get; set; }
    }

    public class KetQuaSinhHieuPtttVo : GridItem
    {
        public int? NhipTim { get; set; }

        public int? NhipTho { get; set; }

        public int? HuyetApTamThu { get; set; }

        public int? HuyetApTamTruong { get; set; }

        public string HuyetAp => DisplayHuyetAp(HuyetApTamThu, HuyetApTamTruong);

        private string DisplayHuyetAp(int? huyetApTamThu, int? huyetApTamTruong)
        {
            if (huyetApTamThu == null && huyetApTamTruong == null)
            {
                return null;
            }

            huyetApTamTruong = huyetApTamTruong.GetValueOrDefault();
            huyetApTamThu = huyetApTamThu.GetValueOrDefault();
            return huyetApTamThu + "/" + huyetApTamTruong;
        }

        public double? ThanNhiet { get; set; }

        public double? ChieuCao { get; set; }

        public double? CanNang { get; set; }

        public double? Bmi { get; set; }

        public double? Glassgow { get; set; }

        public double? SpO2 { get; set; }

        public long? NoiThucHienId { get; set; }

        public long? NhanVienThucHienId { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string NgayThucHienDisplay => NgayThucHien != null ? NgayThucHien.GetValueOrDefault().ApplyFormatDateTimeSACH() : string.Empty;
    }
}
