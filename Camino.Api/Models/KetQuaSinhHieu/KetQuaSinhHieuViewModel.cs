using System.ComponentModel.DataAnnotations.Schema;

namespace Camino.Api.Models.KetQuaSinhHieu
{
    public class KetQuaSinhHieuViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }

        public int? NhipTim { get; set; }

        public int? NhipTho { get; set; }

        public double? ThanNhiet { get; set; }

        public long? NoiThucHienId { get; set; }

        public long NhanVienThucHienId { get; set; }

        public int? HuyetApTamThu { get; set; }

        public int? HuyetApTamTruong { get; set; }

        public string HuyetAp
        {
            get
            {
                return HuyetApTamThu + "/" + HuyetApTamTruong;
            }
        }

        public double? ChieuCao { get; set; }

        public double? CanNang { get; set; }

        public double? BMI { get; set; }
        public double? Glassgow { get; set; }

        public string NgayThucHien { get; set; }
        public double? SpO2 { get; set; }
        [NotMapped]
        public bool IsUpdate { get; set; }
    }
}
