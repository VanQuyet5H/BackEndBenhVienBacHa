using Camino.Core.Domain.Entities.ICDs;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Core.Domain.Entities.TrieuChungs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.Thuocs
{
    public class ToaThuocMau : BaseEntity
    {
        public string Ten { get; set; }
        public long? ICDId { get; set; }
        public long? TrieuChungId { get; set; }
        public long? ChuanDoanId { get; set; }
        public long BacSiKeToaId { get; set; }
        public string GhiChu { get; set; }
        public bool? IsDisabled { get; set; }

        public virtual ICD ICD { get; set; }
        public virtual NhanViens.NhanVien BacSiKeToa { get; set; }
        public virtual ChuanDoan ChuanDoan { get; set; }
        public virtual TrieuChung TrieuChung { get; set; }


        private ICollection<ToaThuocMauChiTiet> _toaThuocMauChiTiets;
        public virtual ICollection<ToaThuocMauChiTiet> ToaThuocMauChiTiets
        {
            get => _toaThuocMauChiTiets ?? (_toaThuocMauChiTiets = new List<ToaThuocMauChiTiet>());
            protected set => _toaThuocMauChiTiets = value;
        }

        private ICollection<YeuCauKhamBenhDonThuoc> _yeuCauKhamBenhDonThuocs;
        public virtual ICollection<YeuCauKhamBenhDonThuoc> YeuCauKhamBenhDonThuocs
        {
            get => _yeuCauKhamBenhDonThuocs ?? (_yeuCauKhamBenhDonThuocs = new List<YeuCauKhamBenhDonThuoc>());
            protected set => _yeuCauKhamBenhDonThuocs = value;
        }

        private ICollection<NoiTruDonThuoc> _noiTruDonThuocs;
        public virtual ICollection<NoiTruDonThuoc> NoiTruDonThuocs
        {
            get => _noiTruDonThuocs ?? (_noiTruDonThuocs = new List<NoiTruDonThuoc>());
            protected set => _noiTruDonThuocs = value;
        }
    }
}
