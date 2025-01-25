using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.Thuocs;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.NoiTruDonThuocs
{
    public class NoiTruDonThuoc : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? ToaThuocMauId { get; set; }
        public EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
        public EnumTrangThaiDonThuoc TrangThai { get; set; }
        public long BacSiKeDonId { get; set; }
        public long NoiKeDonId { get; set; }
        public DateTime ThoiDiemKeDon { get; set; }
        public string GhiChu { get; set; }
        public virtual NhanVien BacSiKeDon { get; set; }
        public virtual PhongBenhVien NoiKeDon { get; set; }
        public virtual ToaThuocMau ToaThuocMau { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }

        private ICollection<NoiTruDonThuocChiTiet> _noiTruDonThuocChiTiets;
        public virtual ICollection<NoiTruDonThuocChiTiet> NoiTruDonThuocChiTiets
        {
            get => _noiTruDonThuocChiTiets ?? (_noiTruDonThuocChiTiets = new List<NoiTruDonThuocChiTiet>());
            protected set => _noiTruDonThuocChiTiets = value;
        }

        private ICollection<DonThuocThanhToan> _donThuocThanhToans;
        public virtual ICollection<DonThuocThanhToan> DonThuocThanhToans
        {
            get => _donThuocThanhToans ?? (_donThuocThanhToans = new List<DonThuocThanhToan>());
            protected set => _donThuocThanhToans = value;
        }
    }
}
