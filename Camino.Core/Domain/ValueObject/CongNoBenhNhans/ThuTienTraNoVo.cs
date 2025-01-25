using Camino.Core.Domain.ValueObject.Grid;
using Camino.Core.Helpers;
using System;

namespace Camino.Core.Domain.ValueObject.CongNoBenhNhans
{
    public class ThongTinTraNoVo : GridItem
    {
        public string TenBenhNhan { get; set; }
        public decimal SoTienChuaThu { get; set; }
    }

    public class ThuTienTraNoVo : GridItem
    {
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? POS { get; set; }
        public DateTime NgayThu { get; set; }
        public string NoiDungThu { get; set; }

        public decimal SoTienChuaThu { get; set; }
        public decimal SoTienThu { get; set; }
    }
}
