using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruPhieuDieuTriChiTietDienBien : BaseEntity
    {
        //public long NoiTruPhieuDieuTriId { get; set; }
        public string MoTaDienBien { get; set; }
        public int GioDienBien { get; set; }
        public DateTime? ThoiDiemCapNhat { get; set; }
        public long? NhanVienCapNhatId { get; set; }

        //BVHD-3312
        public long NoiTruBenhAnId { get; set; }
        public DateTime NgayDieuTri { get; set; }

        //public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }
        public virtual NhanVien NhanVienCapNhat { get; set; }

        //BVHD-3312
        public virtual NoiTruBenhAn NoiTruBenhAn { get; set; }
    }
}
