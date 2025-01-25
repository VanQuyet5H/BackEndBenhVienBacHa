using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.NhapKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauNhapKhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauNhapKhoVatTus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.KhoDuocPhamViTris
{
    public class KhoViTri : BaseEntity
    {
        //public long KhoDuocPhamId { get; set; }
        public long KhoId { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }
        public bool? IsDisabled { get; set; }
        public virtual Kho KhoDuocPham { get; set; }

        private ICollection<NhapKhoVatTuChiTiet> _nhapKhoVatTuChiTiets;
        public virtual ICollection<NhapKhoVatTuChiTiet> NhapKhoVatTuChiTiets
        {
            get => _nhapKhoVatTuChiTiets ?? (_nhapKhoVatTuChiTiets = new List<NhapKhoVatTuChiTiet>());
            protected set => _nhapKhoVatTuChiTiets = value;

        }

        private ICollection<YeuCauNhapKhoDuocPhamChiTiet> _yeuCauNhapKhoDuocPhamChiTiets;
        public virtual ICollection<YeuCauNhapKhoDuocPhamChiTiet> YeuCauNhapKhoDuocPhamChiTiets
        {
            get => _yeuCauNhapKhoDuocPhamChiTiets ?? (_yeuCauNhapKhoDuocPhamChiTiets = new List<YeuCauNhapKhoDuocPhamChiTiet>());
            protected set => _yeuCauNhapKhoDuocPhamChiTiets = value;

        }

        private ICollection<YeuCauNhapKhoVatTuChiTiet> _yeuCauNhapKhoVatTuChiTiets;

        public virtual ICollection<YeuCauNhapKhoVatTuChiTiet> YeuCauNhapKhoVatTuChiTiets
        {
            get => _yeuCauNhapKhoVatTuChiTiets ?? (_yeuCauNhapKhoVatTuChiTiets = new List<YeuCauNhapKhoVatTuChiTiet>());
            protected set => _yeuCauNhapKhoVatTuChiTiets = value;
        }
    }
}
