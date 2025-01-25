using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.XuatKhoVatTus;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauTraVatTuTuBenhNhanChiTiet : BaseEntity
    {
        public long? YeuCauTraVatTuTuBenhNhanId { get; set; }
        public long YeuCauVatTuBenhVienId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public long KhoTraId { get; set; }
        public bool LaVatTuBHYT { get; set; }
        public bool TraVeTuTruc { get; set; }
        public double SoLuongTra { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public long NhanVienYeuCauId { get; set; }

        public virtual YeuCauTraVatTuTuBenhNhan YeuCauTraVatTuTuBenhNhan { get; set; }
        public virtual YeuCauVatTuBenhVien YeuCauVatTuBenhVien { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual Kho KhoTra { get; set; }
        public virtual NhanVien NhanVienYeuCau { get; set; }

        private ICollection<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTris;
        public virtual ICollection<XuatKhoVatTuChiTietViTri> XuatKhoVatTuChiTietViTris
        {
            get => _xuatKhoVatTuChiTietViTris ?? (_xuatKhoVatTuChiTietViTris = new List<XuatKhoVatTuChiTietViTri>());
            protected set => _xuatKhoVatTuChiTietViTris = value;

        }
    }
}
