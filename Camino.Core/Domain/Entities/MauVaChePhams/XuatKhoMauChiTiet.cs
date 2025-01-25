using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.MauVaChePhams
{
    public class XuatKhoMauChiTiet : BaseEntity
    {
        public long XuatKhoMauId { get; set; }
        public long NhapKhoMauChiTietId { get; set; }

        public virtual XuatKhoMau XuatKhoMau { get; set; }
        public virtual NhapKhoMauChiTiet NhapKhoMauChiTiet { get; set; }

        private ICollection<YeuCauTruyenMau> _yeuCauTruyenMaus;
        public virtual ICollection<YeuCauTruyenMau> YeuCauTruyenMaus
        {
            get => _yeuCauTruyenMaus ?? (_yeuCauTruyenMaus = new List<YeuCauTruyenMau>());
            protected set => _yeuCauTruyenMaus = value;
        }
    }
}
