using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.DonThuocThanhToans
{
    public class DonThuocThanhToan : BaseEntity
    {
        public long? YeuCauKhamBenhDonThuocId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? BenhNhanId { get; set; }
        public Enums.EnumLoaiDonThuoc LoaiDonThuoc { get; set; }
        public Enums.TrangThaiDonThuocThanhToan TrangThai { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public long? NhanVienHuyThanhToanId { get; set; }
        public string LyDoHuyThanhToan { get; set; }
        public long? NoiCapThuocId { get; set; }
        public long? NhanVienCapThuocId { get; set; }
        public DateTime? ThoiDiemCapThuoc { get; set; }
        public string GhiChu { get; set; }
        public long? NoiTruDonThuocId { get; set; }
        public virtual YeuCauKhamBenhDonThuoc YeuCauKhamBenhDonThuoc { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual NhanVien NhanVienHuyThanhToan { get; set; }
        public virtual PhongBenhVien NoiCapThuoc { get; set; }
        public virtual NhanVien NhanVienCapThuoc { get; set; }
        //update 15/06/2021
        public virtual NoiTruDonThuoc NoiTruDonThuoc { get; set; }


        private ICollection<DonThuocThanhToanChiTiet> _donThuocThanhToanChiTiets;
        public virtual ICollection<DonThuocThanhToanChiTiet> DonThuocThanhToanChiTiets
        {
            get => _donThuocThanhToanChiTiets ?? (_donThuocThanhToanChiTiets = new List<DonThuocThanhToanChiTiet>());
            protected set => _donThuocThanhToanChiTiets = value;
        }
        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThus;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThus
        {
            get => _taiKhoanBenhNhanThus ?? (_taiKhoanBenhNhanThus = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThus = value;
        }
    }
}
