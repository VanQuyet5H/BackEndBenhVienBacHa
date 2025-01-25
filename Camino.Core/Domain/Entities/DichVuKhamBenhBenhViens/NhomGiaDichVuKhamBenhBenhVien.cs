using Camino.Core.Domain.Entities.PhongBenhViens;
using System;
using System.Collections.Generic;
using System.Text;
 using Camino.Core.Domain.Entities.GoiDichVus;
 using Camino.Core.Domain.Entities.Vouchers;
 using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.NoiGioiThieu;

namespace Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens
{
    
    public class NhomGiaDichVuKhamBenhBenhVien : BaseEntity
    {
        public string Ten { get; set; }
       
        private ICollection<PhongBenhVienNhomGiaDichVuKhamBenh> _phongBenhVienNhomGiaDichVuKhamBenhs { get; set; }
        public virtual ICollection<PhongBenhVienNhomGiaDichVuKhamBenh> PhongBenhVienNhomGiaDichVuKhamBenh
        {
            get => _phongBenhVienNhomGiaDichVuKhamBenhs ?? (_phongBenhVienNhomGiaDichVuKhamBenhs = new List<PhongBenhVienNhomGiaDichVuKhamBenh>());
            protected set => _phongBenhVienNhomGiaDichVuKhamBenhs = value;
        }
        
        private ICollection<DichVuKhamBenhBenhVienGiaBenhVien> _dichVuKhamBenhBenhVienGiaBenhViens { get; set; }
        public virtual ICollection<DichVuKhamBenhBenhVienGiaBenhVien> DichVuKhamBenhBenhVienGiaBenhViens
        {
            get => _dichVuKhamBenhBenhVienGiaBenhViens ?? (_dichVuKhamBenhBenhVienGiaBenhViens = new List<DichVuKhamBenhBenhVienGiaBenhVien>());
            protected set => _dichVuKhamBenhBenhVienGiaBenhViens = value;
        }

        private ICollection<GoiDichVuChiTietDichVuKhamBenh> _goiDichVuChiTietDichVuKhamBenhs { get; set; }
        public virtual ICollection<GoiDichVuChiTietDichVuKhamBenh> GoiDichVuChiTietDichVuKhamBenhs
        {
            get => _goiDichVuChiTietDichVuKhamBenhs ?? (_goiDichVuChiTietDichVuKhamBenhs = new List<GoiDichVuChiTietDichVuKhamBenh>());
            protected set => _goiDichVuChiTietDichVuKhamBenhs = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs { get; set; }
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }

        private ICollection<ChuongTrinhGoiDichVuDichVuKhamBenh> _chuongTrinhGoiDichVuDichKhamBenhs { get; set; }
        public virtual ICollection<ChuongTrinhGoiDichVuDichVuKhamBenh> ChuongTrinhGoiDichVuDichKhamBenhs
        {
            get => _chuongTrinhGoiDichVuDichKhamBenhs ?? (_chuongTrinhGoiDichVuDichKhamBenhs = new List<ChuongTrinhGoiDichVuDichVuKhamBenh>());
            protected set => _chuongTrinhGoiDichVuDichKhamBenhs = value;
        }
        
        private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        {
            get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
            protected set => _voucherChiTietMienGiams = value;
        }

        private ICollection<GoiKhamSucKhoeDichVuKhamBenh> _goiKhamSucKhoeDichVuKhamBenhs;
        public virtual ICollection<GoiKhamSucKhoeDichVuKhamBenh> GoiKhamSucKhoeDichVuKhamBenhs
        {
            get => _goiKhamSucKhoeDichVuKhamBenhs ?? (_goiKhamSucKhoeDichVuKhamBenhs = new List<GoiKhamSucKhoeDichVuKhamBenh>());
            protected set => _goiKhamSucKhoeDichVuKhamBenhs = value;
        }

        private ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs;
        public virtual ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh> ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs
        {
            get => _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs ?? (_chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs = new List<ChuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenh>());
            protected set => _chuongTrinhGoiDichVuKhuyenMaiDichVuKhamBenhs = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuKhamBenh> _goiKhamSucKhoeChungDichVuKhamBenhs;
        public virtual ICollection<GoiKhamSucKhoeChungDichVuKhamBenh> GoiKhamSucKhoeChungDichVuKhamBenhs
        {
            get => _goiKhamSucKhoeChungDichVuKhamBenhs ?? (_goiKhamSucKhoeChungDichVuKhamBenhs = new List<GoiKhamSucKhoeChungDichVuKhamBenh>());
            protected set => _goiKhamSucKhoeChungDichVuKhamBenhs = value;
        }

        private ICollection<NoiGioiThieuChiTietMienGiam> _noiGioiThieuChiTietMienGiams;
        public virtual ICollection<NoiGioiThieuChiTietMienGiam> NoiGioiThieuChiTietMienGiams
        {
            get => _noiGioiThieuChiTietMienGiams ?? (_noiGioiThieuChiTietMienGiams = new List<NoiGioiThieuChiTietMienGiam>());
            protected set => _noiGioiThieuChiTietMienGiams = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh> NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs ?? (_noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKhamBenh>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDichVuKhamBenhs = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh> NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs
        {
            get => _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs ?? (_noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKhamBenh>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDichVuKhamBenhs = value;
        }
    }
}
