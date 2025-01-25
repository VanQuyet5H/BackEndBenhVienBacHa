using Camino.Core.Domain.Entities.PhongBenhViens;
using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuKyThuats;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.NoiGioiThieu;

namespace Camino.Core.Domain.Entities.DichVuKyThuats
{
    public class NhomGiaDichVuKyThuatBenhVien : BaseEntity
    {
        public string Ten { get; set; }
        private ICollection<PhongBenhVienNhomGiaDichVuKyThuat> _phongBenhVienNhomGiaDichVuKyThuats { get; set; }
        public virtual ICollection<PhongBenhVienNhomGiaDichVuKyThuat> PhongBenhVienNhomGiaDichVuKyThuats
        {
            get => _phongBenhVienNhomGiaDichVuKyThuats ?? (_phongBenhVienNhomGiaDichVuKyThuats = new List<PhongBenhVienNhomGiaDichVuKyThuat>());
            protected set => _phongBenhVienNhomGiaDichVuKyThuats = value;
        }
        
        
        private ICollection<DichVuKyThuatBenhVienGiaBenhVien> _dichVuKyThuatVuBenhVienGiaBenhViens { get; set; }
        public virtual ICollection<DichVuKyThuatBenhVienGiaBenhVien> DichVuKyThuatVuBenhVienGiaBenhViens
        {
            get => _dichVuKyThuatVuBenhVienGiaBenhViens ?? (_dichVuKyThuatVuBenhVienGiaBenhViens = new List<DichVuKyThuatBenhVienGiaBenhVien>());
            protected set => _dichVuKyThuatVuBenhVienGiaBenhViens = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats { get; set; }
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }

        private ICollection<GoiDichVuChiTietDichVuKyThuat> _goiDichVuChiTietDichVuKyThuats { get; set; }
        public virtual ICollection<GoiDichVuChiTietDichVuKyThuat> GoiDichVuChiTietDichVuKyThuats
        {
            get => _goiDichVuChiTietDichVuKyThuats ?? (_goiDichVuChiTietDichVuKyThuats = new List<GoiDichVuChiTietDichVuKyThuat>());
            protected set => _goiDichVuChiTietDichVuKyThuats = value;
        }

        private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        {
            get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
            protected set => _voucherChiTietMienGiams = value;
        }
        
        private ICollection<ChuongTrinhGoiDichVuDichVuKyThuat> _chuongTrinhGoiDichVuDichVuKyThuats { get; set; }
        public virtual ICollection<ChuongTrinhGoiDichVuDichVuKyThuat> ChuongTrinhGoiDichVuDichVuKyThuats
        {
            get => _chuongTrinhGoiDichVuDichVuKyThuats ?? (_chuongTrinhGoiDichVuDichVuKyThuats = new List<ChuongTrinhGoiDichVuDichVuKyThuat>());
            protected set => _chuongTrinhGoiDichVuDichVuKyThuats = value;
        }

        private ICollection<GoiKhamSucKhoeDichVuDichVuKyThuat> _goiKhamSucKhoeDichVuDichVuKyThuats;
        public virtual ICollection<GoiKhamSucKhoeDichVuDichVuKyThuat> GoiKhamSucKhoeDichVuDichVuKyThuats
        {
            get => _goiKhamSucKhoeDichVuDichVuKyThuats ?? (_goiKhamSucKhoeDichVuDichVuKyThuats = new List<GoiKhamSucKhoeDichVuDichVuKyThuat>());
            protected set => _goiKhamSucKhoeDichVuDichVuKyThuats = value;
        }

        private ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats;
        public virtual ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat> ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats
        {
            get => _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats ?? (_chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats = new List<ChuongTrinhGoiDichVuKhuyenMaiDichVuKyThuat>());
            protected set => _chuongTrinhGoiDichVuKhuyenMaiDichVuKyThuats = value;
        }

        private ICollection<GoiKhamSucKhoeChungDichVuDichVuKyThuat> _goiKhamSucKhoeChungDichVuDichVuKyThuats;
        public virtual ICollection<GoiKhamSucKhoeChungDichVuDichVuKyThuat> GoiKhamSucKhoeChungDichVuDichVuKyThuats
        {
            get => _goiKhamSucKhoeChungDichVuDichVuKyThuats ?? (_goiKhamSucKhoeChungDichVuDichVuKyThuats = new List<GoiKhamSucKhoeChungDichVuDichVuKyThuat>());
            protected set => _goiKhamSucKhoeChungDichVuDichVuKyThuats = value;
        }

        private ICollection<NoiGioiThieuChiTietMienGiam> _noiGioiThieuChiTietMienGiams;
        public virtual ICollection<NoiGioiThieuChiTietMienGiam> NoiGioiThieuChiTietMienGiams
        {
            get => _noiGioiThieuChiTietMienGiams ?? (_noiGioiThieuChiTietMienGiams = new List<NoiGioiThieuChiTietMienGiam>());
            protected set => _noiGioiThieuChiTietMienGiams = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat> NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuats
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats ?? (_noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuKyThuat>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDichVuKyThuats = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> _noiGioiThieuHopDongChiTietHeSoDichVuKyThuats;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat> NoiGioiThieuHopDongChiTietHeSoDichVuKyThuats
        {
            get => _noiGioiThieuHopDongChiTietHeSoDichVuKyThuats ?? (_noiGioiThieuHopDongChiTietHeSoDichVuKyThuats = new List<NoiGioiThieuHopDongChiTietHeSoDichVuKyThuat>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDichVuKyThuats = value;
        }
    }
}
