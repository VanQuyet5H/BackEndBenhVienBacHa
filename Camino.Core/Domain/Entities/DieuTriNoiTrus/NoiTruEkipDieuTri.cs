using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruEkipDieuTri : BaseEntity
    {
        public long NoiTruBenhAnId { get; set; }
        public long BacSiId { get; set; }
        public long DieuDuongId { get; set; }
        public long NhanVienLapId { get; set; }
        public long KhoaPhongDieuTriId { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }

        public virtual NoiTruBenhAn NoiTruBenhAn { get; set; }
        public virtual NhanVien BacSi { get; set; }
        public virtual NhanVien DieuDuong { get; set; }
        public virtual NhanVien NhanVienLap { get; set; }
        public virtual KhoaPhong KhoaPhongDieuTri { get; set; }
    }
}
