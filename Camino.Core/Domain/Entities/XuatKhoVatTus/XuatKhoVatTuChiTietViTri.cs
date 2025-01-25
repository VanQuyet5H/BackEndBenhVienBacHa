using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.YeuCauTraVatTus;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.XuatKhoVatTus
{
    public class XuatKhoVatTuChiTietViTri : BaseEntity
    {
        public long XuatKhoVatTuChiTietId { get; set; }
        public long NhapKhoVatTuChiTietId { get; set; }
        public double SoLuongXuat { get; set; }
        public DateTime? NgayXuat { get; set; }
        public long? YeuCauTraVatTuTuBenhNhanChiTietId { get; set; }
        public string GhiChu { get; set; }

        public virtual XuatKhoVatTuChiTiet XuatKhoVatTuChiTiet { get; set; }
        public virtual NhapKhoVatTuChiTiet NhapKhoVatTuChiTiet { get; set; }
        public virtual YeuCauTraVatTuTuBenhNhanChiTiet YeuCauTraVatTuTuBenhNhanChiTiet { get; set; }

        private ICollection<YeuCauTraVatTuChiTiet> _yeuCauTraVatTuChiTiets;
        public virtual ICollection<YeuCauTraVatTuChiTiet> YeuCauTraVatTuChiTiets
        {
            get => _yeuCauTraVatTuChiTiets ?? (_yeuCauTraVatTuChiTiets = new List<YeuCauTraVatTuChiTiet>());
            protected set => _yeuCauTraVatTuChiTiets = value;
        }
        private ICollection<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTiets;
        public virtual ICollection<DonVTYTThanhToanChiTiet> DonVTYTThanhToanChiTiets
        {
            get => _donVTYTThanhToanChiTiets ?? (_donVTYTThanhToanChiTiets = new List<DonVTYTThanhToanChiTiet>());
            protected set => _donVTYTThanhToanChiTiets = value;
        }

        private ICollection<YeuCauXuatKhoVatTuChiTiet> _yeuCauXuatKhoVatTuChiTiets;
        public virtual ICollection<YeuCauXuatKhoVatTuChiTiet> YeuCauXuatKhoVatTuChiTiets
        {
            get => _yeuCauXuatKhoVatTuChiTiets ?? (_yeuCauXuatKhoVatTuChiTiets = new List<YeuCauXuatKhoVatTuChiTiet>());
            protected set => _yeuCauXuatKhoVatTuChiTiets = value;
        }
    }
}
