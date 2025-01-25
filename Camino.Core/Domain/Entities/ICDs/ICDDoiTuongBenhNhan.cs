using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.ICDs
{
    public class ICDDoiTuongBenhNhan : BaseEntity
    {
        public string Ten { get; set; }
        public string GhiChu { get; set; }

        public ICollection<ICDDoiTuongBenhNhanChiTiet> _iCDDoiTuongBenhNhanChiTiests;
        public virtual ICollection<ICDDoiTuongBenhNhanChiTiet> ICDDoiTuongBenhNhanChiTiets
        {
            get => _iCDDoiTuongBenhNhanChiTiests ?? (_iCDDoiTuongBenhNhanChiTiests = new List<ICDDoiTuongBenhNhanChiTiet>());
            protected set => _iCDDoiTuongBenhNhanChiTiests = value;
        }
    }
}
