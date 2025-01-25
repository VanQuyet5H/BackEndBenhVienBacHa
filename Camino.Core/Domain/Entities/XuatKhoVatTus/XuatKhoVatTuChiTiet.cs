using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTraDuocPhams;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.XuatKhoVatTus
{
    public class XuatKhoVatTuChiTiet : BaseEntity
    {
        public long? XuatKhoVatTuId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public DateTime? NgayXuat { get; set; }
        public virtual XuatKhoVatTu XuatKhoVatTu { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }

        private ICollection<XuatKhoVatTuChiTietViTri> _xuatKhoVatTuChiTietViTris;
        public virtual ICollection<XuatKhoVatTuChiTietViTri> XuatKhoVatTuChiTietViTris
        {
            get => _xuatKhoVatTuChiTietViTris ?? (_xuatKhoVatTuChiTietViTris = new List<XuatKhoVatTuChiTietViTri>());
            protected set => _xuatKhoVatTuChiTietViTris = value;

        }

        private ICollection<YeuCauTraDuocPham> _yeuCauTraDuocPhams;

        //public virtual ICollection<YeuCauTraDuocPham> YeuCauTraDuocPhams
        //{
        //    get => _yeuCauTraDuocPhams ?? (_yeuCauTraDuocPhams = new List<YeuCauTraDuocPham>());
        //    protected set => _yeuCauTraDuocPhams = value;
        //}
        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }
    }
}
