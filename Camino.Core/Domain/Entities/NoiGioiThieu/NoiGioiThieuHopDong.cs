using System;
using System.Collections.Generic;
using System.Text;

namespace Camino.Core.Domain.Entities.NoiGioiThieu
{
    public class NoiGioiThieuHopDong : BaseEntity
    {
        public long NoiGioiThieuId { get; set; }
        public string Ten { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }

        public virtual NoiGioiThieu NoiGioiThieu { get; set; }
                
        private ICollection<NoiGioiThieuHopDongChiTietHoaHongVatTu> _noiGioiThieuHopDongChiTietHoaHongVatTus;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongVatTu> NoiGioiThieuHopDongChiTietHoaHongVatTus
        {
            get => _noiGioiThieuHopDongChiTietHoaHongVatTus ?? (_noiGioiThieuHopDongChiTietHoaHongVatTus = new List<NoiGioiThieuHopDongChiTietHoaHongVatTu>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongVatTus = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDuocPham> _noiGioiThieuHopDongChiTietHoaHongDuocPhams;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDuocPham> NoiGioiThieuHopDongChiTietHoaHongDuocPhams
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDuocPhams ?? (_noiGioiThieuHopDongChiTietHoaHongDuocPhams = new List<NoiGioiThieuHopDongChiTietHoaHongDuocPham>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDuocPhams = value;
        }
        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs ?? (_noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs = value;
        }
        
        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuats
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats ?? (_noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats = value;
        }
        
        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong> _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong> NoiGioiThieuHopDongChiTietHoaHongDichVuGiuongs
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs ?? (_noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs = value;
        }
        
        private ICollection<NoiGioiThieuHopDongChiTietHeSoVatTu> _noiGioiThieuHopDongChiTietHeSoVatTus;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoVatTu> NoiGioiThieuHopDongChiTietHeSoVatTus
        {
            get => _noiGioiThieuHopDongChiTietHeSoVatTus ?? (_noiGioiThieuHopDongChiTietHeSoVatTus = new List<NoiGioiThieuHopDongChiTietHeSoVatTu>());
            protected set => _noiGioiThieuHopDongChiTietHeSoVatTus = value;
        }
        
        private ICollection<NoiGioiThieuHopDongChiTietHeSoDuocPham> _noiGioiThieuHopDongChiTietHeSoDuocPhams;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDuocPham> NoiGioiThieuHopDongChiTietHeSoDuocPhams
        {
            get => _noiGioiThieuHopDongChiTietHeSoDuocPhams ?? (_noiGioiThieuHopDongChiTietHeSoDuocPhams = new List<NoiGioiThieuHopDongChiTietHeSoDuocPham>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDuocPhams = value;
        }
        
        private ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs
        {
            get => _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs ?? (_noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs = value;
        }
        
        private ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> _noiGioiThieuHopDongChiTietHeSoDichVuKyThuats;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> NoiGioiThieuHopDongChiTietHeSoDichVuKyThuats
        {
            get => _noiGioiThieuHopDongChiTietHeSoDichVuKyThuats ?? (_noiGioiThieuHopDongChiTietHeSoDichVuKyThuats = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDichVuKyThuats = value;
        }
        
        private ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong> _noiGioiThieuHopDongChiTietHeSoDichVuGiuongs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong> NoiGioiThieuHopDongChiTietHeSoDichVuGiuongs
        {
            get => _noiGioiThieuHopDongChiTietHeSoDichVuGiuongs ?? (_noiGioiThieuHopDongChiTietHeSoDichVuGiuongs = new List<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDichVuGiuongs = value;
        }
    }
}
