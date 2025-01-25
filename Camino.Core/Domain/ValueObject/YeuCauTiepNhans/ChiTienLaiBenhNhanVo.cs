using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class ChiTienLaiBenhNhanVo
    {
        public long Id { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public DateTime NgayChi { get; set; }
        public string NoiDungChiTien { get; set; }
    }

}
