using Camino.Core.Domain.Entities.VatTus;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.NhomVatTus
{
    public class NhomVatTu : BaseEntity
    {
        public string Ma { get; set; }
        public string Ten { get; set; }
        public long? NhomVatTuChaId { get; set; }
        public int CapNhom { get; set; }
        private ICollection<VatTu> _vatTus;
        public virtual ICollection<VatTu> VatTus
        {
            get => _vatTus ?? (_vatTus = new List<VatTu>());
            protected set => _vatTus = value;
        }

        public virtual NhomVatTu nhomVatTu { get; set; }

        private ICollection<NhomVatTu> _nhomVatTus;
        public virtual ICollection<NhomVatTu> NhomVatTus
        {
            get => _nhomVatTus ?? (_nhomVatTus = new List<NhomVatTu>());
            protected set => _nhomVatTus = value;
        }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens { get; set; }
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }

        private ICollection<YeuCauKhamBenhDonVTYTChiTiet> _yeuCauKhamBenhDonVTYTChiTiets;
        public virtual ICollection<YeuCauKhamBenhDonVTYTChiTiet> YeuCauKhamBenhDonVTYTChiTiets
        {
            get => _yeuCauKhamBenhDonVTYTChiTiets ?? (_yeuCauKhamBenhDonVTYTChiTiets = new List<YeuCauKhamBenhDonVTYTChiTiet>());
            protected set => _yeuCauKhamBenhDonVTYTChiTiets = value;
        }

        private ICollection<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTiets;
        public virtual ICollection<DonVTYTThanhToanChiTiet> DonVTYTThanhToanChiTiets
        {
            get => _donVTYTThanhToanChiTiets ?? (_donVTYTThanhToanChiTiets = new List<DonVTYTThanhToanChiTiet>());
            protected set => _donVTYTThanhToanChiTiets = value;
        }
    }

}
