using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.DonVatTus
{
    public class YeuCauKhamBenhDonVTYT : BaseEntity
    {
        public long YeuCauKhamBenhId { get; set; }
        public Enums.EnumTrangThaiDonVTYT TrangThai { get; set; }
        public long BacSiKeDonId { get; set; }
        public long NoiKeDonId { get; set; }
        public DateTime ThoiDiemKeDon { get; set; }
        public string GhiChu { get; set; }


        public virtual NhanVien BacSiKeDon { get; set; }
        public virtual PhongBenhVien NoiKeDon { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }

        private ICollection<YeuCauKhamBenhDonVTYTChiTiet> _yeuCauKhamBenhDonVTYTChiTiets;
        public virtual ICollection<YeuCauKhamBenhDonVTYTChiTiet> YeuCauKhamBenhDonVTYTChiTiets
        {
            get => _yeuCauKhamBenhDonVTYTChiTiets ?? (_yeuCauKhamBenhDonVTYTChiTiets = new List<YeuCauKhamBenhDonVTYTChiTiet>());
            protected set => _yeuCauKhamBenhDonVTYTChiTiets = value;
        }

        private ICollection<DonVTYTThanhToan> _donVTYTThanhToans;
        public virtual ICollection<DonVTYTThanhToan> DonVTYTThanhToans
        {
            get => _donVTYTThanhToans ?? (_donVTYTThanhToans = new List<DonVTYTThanhToan>());
            protected set => _donVTYTThanhToans = value;
        }
    }
}
