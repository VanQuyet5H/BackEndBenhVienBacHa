using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.MauVaChePhams
{
    public class MauVaChePham : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public Enums.PhanLoaiMau PhanLoaiMau { get; set; }
        public long TheTich { get; set; }
        public long GiaTriToiDa { get; set; }
        public string GhiChu { get; set; }

        private ICollection<NhapKhoMauChiTiet> _nhapKhoMauChiTiets;
        public virtual ICollection<NhapKhoMauChiTiet> NhapKhoMauChiTiets
        {
            get => _nhapKhoMauChiTiets ?? (_nhapKhoMauChiTiets = new List<NhapKhoMauChiTiet>());
            protected set => _nhapKhoMauChiTiets = value;
        }

        private ICollection<YeuCauTruyenMau> _yeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> YeuCauTruyenMaus
        {
            get => _yeuCauTruyenMaus ?? (_yeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _yeuCauTruyenMaus = value;
        }
    }
}
