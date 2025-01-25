using Camino.Core.Domain.Entities.FileKetQuaCanLamSangs;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.KetQuaNhomXetNghiems
{
    public class KetQuaNhomXetNghiem : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long NhomDichVuBenhVienId { get; set; }
        //public long FileKetQuaCanLamSangId { get; set; }
        public long? NhanVienKetLuanId { get; set; }
        public string KetLuan { get; set; }
        public DateTime? ThoiDiemKetLuan { get; set; }
      
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        //public virtual FileKetQuaCanLamSang FileKetQuaCanLamSang { get; set; }
        public virtual NhomDichVuBenhVien.NhomDichVuBenhVien NhomDichVuBenhVien { get; set; }

        public virtual NhanVien NhanVienKetLuan { get; set; }

        private ICollection<FileKetQuaCanLamSang> _fileKetQuaCanLamSangs { get; set; }
        public virtual ICollection<FileKetQuaCanLamSang> FileKetQuaCanLamSangs
        {
            get => _fileKetQuaCanLamSangs ?? (_fileKetQuaCanLamSangs = new List<FileKetQuaCanLamSang>());
            protected set => _fileKetQuaCanLamSangs = value;
        }
    }
}
