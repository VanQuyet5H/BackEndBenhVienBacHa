using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.YeuCauNhapViens
{
    public class YeuCauNhapVien : BaseEntity
    {
        public long BenhNhanId { get; set; }
        public long BacSiChiDinhId { get; set; }
        public long NoiChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long KhoaPhongNhapVienId { get; set; }
        public string LyDoNhapVien { get; set; }
        public bool LaCapCuu { get; set; }
        public long? ChanDoanNhapVienICDId { get; set; }
        public string ChanDoanNhapVienGhiChu { get; set; }
        public long? YeuCauTiepNhanMeId { get; set; }

        public virtual BenhNhan BenhNhan { get; set; }
        public virtual NhanVien BacSiChiDinh { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual KhoaPhong KhoaPhongNhapVien { get; set; }
        public virtual ICD ChanDoanNhapVienICD { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhanMe { get; set; }

        private ICollection<YeuCauNhapVienChanDoanKemTheo> _yeuCauNhapVienChanDoanKemTheos;
        public virtual ICollection<YeuCauNhapVienChanDoanKemTheo> YeuCauNhapVienChanDoanKemTheos
        {
            get => _yeuCauNhapVienChanDoanKemTheos ?? (_yeuCauNhapVienChanDoanKemTheos = new List<YeuCauNhapVienChanDoanKemTheo>());
            protected set => _yeuCauNhapVienChanDoanKemTheos = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }
    }
}
