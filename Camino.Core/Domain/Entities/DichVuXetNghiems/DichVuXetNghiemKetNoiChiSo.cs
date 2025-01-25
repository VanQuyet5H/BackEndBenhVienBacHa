using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.MauMayXetNghiems;
using Camino.Core.Domain.Entities.XetNghiems;

namespace Camino.Core.Domain.Entities.DichVuXetNghiems
{
    public class DichVuXetNghiemKetNoiChiSo : BaseEntity
    {
        public string MaKetNoi { get; set; }
        public string TenKetNoi { get; set; }
        public long DichVuXetNghiemId { get; set; }
        public string MaChiSo { get; set; }
        public long MauMayXetNghiemId { get; set; }
        public bool HieuLuc { get; set; }
        public double TiLe { get; set; }
        public string MoTa { get; set; }
        public bool? NotSendOrder { get; set; }

        public virtual DichVuXetNghiem DichVuXetNghiem { get; set; }
        public virtual MauMayXetNghiem MauMayXetNghiem { get; set; }

        private ICollection<KetQuaXetNghiemChiTiet> _ketQuaXetNghiemChiTiets;
        public virtual ICollection<KetQuaXetNghiemChiTiet> KetQuaXetNghiemChiTiets
        {
            get => _ketQuaXetNghiemChiTiets ?? (_ketQuaXetNghiemChiTiets = new List<KetQuaXetNghiemChiTiet>());
            protected set => _ketQuaXetNghiemChiTiets = value;
        }
    }
}
