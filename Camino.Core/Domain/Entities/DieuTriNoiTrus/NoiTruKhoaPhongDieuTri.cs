using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.NhanViens;

namespace Camino.Core.Domain.Entities.DieuTriNoiTrus
{
    public class NoiTruKhoaPhongDieuTri : BaseEntity
    {
        public long NoiTruBenhAnId { get; set; }
        public long? KhoaPhongChuyenDiId { get; set; }
        public long KhoaPhongChuyenDenId { get; set; }
        public DateTime ThoiDiemVaoKhoa { get; set; }
        public DateTime? ThoiDiemRaKhoa { get; set; }
        public long? ChanDoanVaoKhoaICDId { get; set; }
        public string ChanDoanVaoKhoaGhiChu { get; set; }
        public string LyDoChuyenKhoa { get; set; }
        public long NhanVienChiDinhId { get; set; }

        public virtual NoiTruBenhAn NoiTruBenhAn { get; set; }
        public virtual KhoaPhong KhoaPhongChuyenDi { get; set; }
        public virtual KhoaPhong KhoaPhongChuyenDen { get; set; }
        public virtual ICD ChanDoanVaoKhoaICD { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
    }
}
