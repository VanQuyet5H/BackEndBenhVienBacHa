using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.Thuocs;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DonThuocThanhToans;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauKhamBenhDonThuoc : BaseEntity
    {
        public long YeuCauKhamBenhId { get; set; }
        public long? ToaThuocMauId { get; set; }
        public Enums.EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
        public Enums.EnumTrangThaiDonThuoc TrangThai { get; set; }
        public long BacSiKeDonId { get; set; }
        public long NoiKeDonId { get; set; }
        public DateTime ThoiDiemKeDon { get; set; }
        public string GhiChu { get; set; }


        public virtual NhanVien BacSiKeDon { get; set; }
        public virtual PhongBenhVien NoiKeDon { get; set; }
        public virtual ToaThuocMau ToaThuocMau { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }

        private ICollection<YeuCauKhamBenhDonThuocChiTiet> _yeuCauKhamBenhDonThuocChiTiets;
        public virtual ICollection<YeuCauKhamBenhDonThuocChiTiet> YeuCauKhamBenhDonThuocChiTiets
        {
            get => _yeuCauKhamBenhDonThuocChiTiets ?? (_yeuCauKhamBenhDonThuocChiTiets = new List<YeuCauKhamBenhDonThuocChiTiet>());
            protected set => _yeuCauKhamBenhDonThuocChiTiets = value;
        }

        private ICollection<DonThuocThanhToan> _donThuocThanhToans;
        public virtual ICollection<DonThuocThanhToan> DonThuocThanhToans
        {
            get => _donThuocThanhToans ?? (_donThuocThanhToans = new List<DonThuocThanhToan>());
            protected set => _donThuocThanhToans = value;
        }
    }
}
