using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.DuyetBaoHiems
{
    public class DuyetBaoHiem : BaseEntity
    {
        public long NhanVienDuyetBaoHiemId { get; set; }
        public long? NoiDuyetBaoHiemId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime ThoiDiemDuyetBaoHiem { get; set; }

        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }
        public virtual PhongBenhVien NoiDuyetBaoHiem { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }

        private ICollection<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTiets;
        public virtual ICollection<DuyetBaoHiemChiTiet> DuyetBaoHiemChiTiets
        {
            get => _duyetBaoHiemChiTiets ?? (_duyetBaoHiemChiTiets = new List<DuyetBaoHiemChiTiet>());
            protected set => _duyetBaoHiemChiTiets = value;
        }
    }
}
