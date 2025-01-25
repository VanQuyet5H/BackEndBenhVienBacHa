using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonViTinhs;
using Camino.Core.Domain.Entities.NhomVatTus;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.DonVatTus
{
    public class YeuCauKhamBenhDonVTYTChiTiet : BaseEntity
    {
        public long YeuCauKhamBenhDonVTYTId { get; set; }
        public long VatTuBenhVienId { get; set; }

        public string Ten { get; set; }
        public string Ma { get; set; }
        public long NhomVatTuId { get; set; }

        public string DonViTinh { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string MoTa { get; set; }
        
        public double SoLuong { get; set; }
        public string GhiChu { get; set; }
        //public int? ThoiGianDungSang { get; set; }
        //public int? ThoiGianDungTrua { get; set; }
        //public int? ThoiGianDungChieu { get; set; }
        //public int? ThoiGianDungToi { get; set; }

        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual YeuCauKhamBenhDonVTYT YeuCauKhamBenhDonVTYT { get; set; }
        public virtual NhomVatTu NhomVatTu { get; set; }

        private ICollection<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTiets;
        public virtual ICollection<DonVTYTThanhToanChiTiet> DonVTYTThanhToanChiTiets
        {
            get => _donVTYTThanhToanChiTiets ?? (_donVTYTThanhToanChiTiets = new List<DonVTYTThanhToanChiTiet>());
            protected set => _donVTYTThanhToanChiTiets = value;
        }
    }
}
