using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.DonVatTus
{
    public class DonVTYTThanhToan : BaseEntity
    {
        public long? YeuCauKhamBenhDonVTYTId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public Enums.TrangThaiDonVTYTThanhToan TrangThai { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public long? NhanVienHuyThanhToanId { get; set; }
        public string LyDoHuyThanhToan { get; set; }
        public long? NoiCapVTYTId { get; set; }
        public long? NhanVienCapVTYTId { get; set; }
        public DateTime? ThoiDiemCapVTYT { get; set; }
        public string GhiChu { get; set; }

        public virtual YeuCauKhamBenhDonVTYT YeuCauKhamBenhDonVTYT { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual NhanVien NhanVienHuyThanhToan { get; set; }
        public virtual PhongBenhVien NoiCapVTYT { get; set; }
        public virtual NhanVien NhanVienCapVTYT { get; set; }

        private ICollection<DonVTYTThanhToanChiTiet> _donVTYTThanhToanChiTiets;
        public virtual ICollection<DonVTYTThanhToanChiTiet> DonVTYTThanhToanChiTiets
        {
            get => _donVTYTThanhToanChiTiets ?? (_donVTYTThanhToanChiTiets = new List<DonVTYTThanhToanChiTiet>());
            protected set => _donVTYTThanhToanChiTiets = value;
        }
        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThus;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThus
        {
            get => _taiKhoanBenhNhanThus ?? (_taiKhoanBenhNhanThus = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThus = value;
        }
    }
}
