using Camino.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruThoiGianDieuTriBenhAnSoSinh : BaseEntity
    {
        public long NoiTruBenhAnId { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }
        public DateTime NgayDieuTri { get; set; }       
        public int? GioBatDau { get; set; }
        public int? GioKetThuc { get; set; }
        public string GhiChuDieuTri { get; set; }
        public virtual NoiTruBenhAn NoiTruBenhAn { get; set; }
        public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }
    }
}
