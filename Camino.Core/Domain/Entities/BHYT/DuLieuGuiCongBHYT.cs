using System.Collections.Generic;

namespace Camino.Core.Domain.Entities.BHYT
{
    public class DuLieuGuiCongBHYT : BaseEntity
    {
        public string DuLieuTongHop { get; set; }
        private ICollection<YeuCauTiepNhanDuLieuGuiCongBHYT> _yeuCauTiepNhanDuLieuGuiCongBHYT;
        public virtual ICollection<YeuCauTiepNhanDuLieuGuiCongBHYT> YeuCauTiepNhanDuLieuGuiCongBHYTs
        {
            get => _yeuCauTiepNhanDuLieuGuiCongBHYT ?? (_yeuCauTiepNhanDuLieuGuiCongBHYT = new List<YeuCauTiepNhanDuLieuGuiCongBHYT>());
            protected set => _yeuCauTiepNhanDuLieuGuiCongBHYT = value;

        }
    }
}