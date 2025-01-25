using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.ChiSoXetNghiems
{
    public class ChiSoXetNghiem : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string ChiSoBinhThuongNam { get; set; }
        public string ChiSoBinhThuongNu { get; set; }
        public Domain.Enums.EnumLoaiXetNghiem LoaiXetNghiem { get; set; }
        public string MoTa { get; set; }
        public bool HieuLuc { get; set; }


        private ICollection<KetQuaXetNghiem> _ketQuaXetNghiems;
        public virtual ICollection<KetQuaXetNghiem> KetQuaXetNghiems
        {
            get => _ketQuaXetNghiems ?? (_ketQuaXetNghiems = new List<KetQuaXetNghiem>());
            protected set => _ketQuaXetNghiems = value;

        }
    }
}
