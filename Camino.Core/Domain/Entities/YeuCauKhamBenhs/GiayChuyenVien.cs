using System.Collections.Generic;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class GiayChuyenVien : TaiLieuDinhKemEntity
    {
       // public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        #region Update 12/2/2020

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhans;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhans
        {
            get => _yeuCauTiepNhans ?? (_yeuCauTiepNhans = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhans = value;
        }

        #endregion Update 12/2/2020
    }
}