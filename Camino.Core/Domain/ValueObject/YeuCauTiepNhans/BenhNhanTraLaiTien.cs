using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.ValueObject.YeuCauTiepNhans
{
    public class BenhNhanTraLaiTien
    {
        public long Id { get; set; }

        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? Pos { get; set; }

        public DateTime NgayTraTien { get; set; }
        public string NoiDungBenhNhanDuaTien { get; set; }
    }

}
