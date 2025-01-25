using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ThongTinThanhToanThuChiVo
    {
        public string ThuChiTienBenhNhanStr { get; set; }
        public string NgayThuStr { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? POS { get; set; }       
        public decimal? SoTienGiam { get; set; }
        public decimal? SoTienCongNo { get; set; }
        public string NoiDungThu { get; set; }
    }
}
