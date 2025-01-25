using System;

namespace Camino.Core.Domain.ValueObject.DieuTriNoiTru
{
    public class TomTatHoSoBenhAnVo
    {
        public string DienBienLamSang { get; set; }

        public string KqXnCls { get; set; }

        public string PpDieuTri { get; set; }

        public string TinhTrangChuyenTuyen { get; set; }

        public string GhiChu { get; set; }

        public DateTime? NgayThucHien { get; set; }

        public string GiamDoc { get; set; }

        public string NguoiThucHienReadonly { get; set; }

        public string NgayThucHienReadonly { get; set; }
    }
}
