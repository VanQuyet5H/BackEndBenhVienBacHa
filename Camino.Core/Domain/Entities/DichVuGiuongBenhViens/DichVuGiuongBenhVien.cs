using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVuDichVuGiuongs;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.DichVuGiuongs;
using Camino.Core.Domain.Entities.DichVuKhamBenhBenhViens;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.KhoaPhongs;
using Camino.Core.Domain.Entities.NoiGioiThieu;
using Camino.Core.Domain.Entities.Vouchers;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;

namespace Camino.Core.Domain.Entities.DichVuGiuongBenhViens
{
    public class DichVuGiuongBenhVien : BaseEntity
    {
        public long? DichVuGiuongId { get; set; }
        public Enums.EnumLoaiGiuong LoaiGiuong { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string MoTa { get; set; }

        public bool HieuLuc { get; set; }

        //BVHD-3905
        public string TiLeThanhToanBHYT { get; set; }

        public virtual DichVuGiuong DichVuGiuong { get; set; }

        private ICollection<DichVuGiuongBenhVienGiaBaoHiem> _dichVuGiuongBenhVienGiaBaoHiems { get; set; }
        public virtual ICollection<DichVuGiuongBenhVienGiaBaoHiem> DichVuGiuongBenhVienGiaBaoHiems
        {
            get => _dichVuGiuongBenhVienGiaBaoHiems ?? (_dichVuGiuongBenhVienGiaBaoHiems = new List<DichVuGiuongBenhVienGiaBaoHiem>());
            protected set => _dichVuGiuongBenhVienGiaBaoHiems = value;
        }
        private ICollection<DichVuGiuongBenhVienGiaBenhVien> _dichVuGiuongVuBenhVienGiaBenhViens { get; set; }
        public virtual ICollection<DichVuGiuongBenhVienGiaBenhVien> DichVuGiuongBenhVienGiaBenhViens
        {
            get => _dichVuGiuongVuBenhVienGiaBenhViens ?? (_dichVuGiuongVuBenhVienGiaBenhViens = new List<DichVuGiuongBenhVienGiaBenhVien>());
            protected set => _dichVuGiuongVuBenhVienGiaBenhViens = value;
        }

        private ICollection<GoiDichVuChiTietDichVuGiuong> _goiDichVuChiTietDichVuGiuongs;
        public virtual ICollection<GoiDichVuChiTietDichVuGiuong> GoiDichVuChiTietDichVuGiuongs
        {
            get => _goiDichVuChiTietDichVuGiuongs ?? (_goiDichVuChiTietDichVuGiuongs = new List<GoiDichVuChiTietDichVuGiuong>());
            protected set => _goiDichVuChiTietDichVuGiuongs = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhViens
        {
            get => _yeuCauDichVuGiuongBenhViens ?? (_yeuCauDichVuGiuongBenhViens = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhViens = value;

        }

        //private ICollection<VoucherChiTietMienGiam> _voucherChiTietMienGiams { get; set; }
        //public virtual ICollection<VoucherChiTietMienGiam> VoucherChiTietMienGiams
        //{
        //    get => _voucherChiTietMienGiams ?? (_voucherChiTietMienGiams = new List<VoucherChiTietMienGiam>());
        //    protected set => _voucherChiTietMienGiams = value;
        //}

        private ICollection<DichVuGiuongBenhVienNoiThucHien> _dichVuGiuongBenhVienNoiThucHiens { get; set; }
        public virtual ICollection<DichVuGiuongBenhVienNoiThucHien> DichVuGiuongBenhVienNoiThucHiens
        {
            get => _dichVuGiuongBenhVienNoiThucHiens ?? (_dichVuGiuongBenhVienNoiThucHiens = new List<DichVuGiuongBenhVienNoiThucHien>());
            protected set => _dichVuGiuongBenhVienNoiThucHiens = value;
        }

        private ICollection<ChuongTrinhGoiDichVuDichVuGiuong> _chuongTrinhGoiDichVuDichVuGiuongs { get; set; }
        public virtual ICollection<ChuongTrinhGoiDichVuDichVuGiuong> ChuongTrinhGoiDichVuDichVuGiuongs
        {
            get => _chuongTrinhGoiDichVuDichVuGiuongs ?? (_chuongTrinhGoiDichVuDichVuGiuongs = new List<ChuongTrinhGoiDichVuDichVuGiuong>());
            protected set => _chuongTrinhGoiDichVuDichVuGiuongs = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVienChiPhi> _yeuCauDichVuGiuongBenhVienChiPhis;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhi> YeuCauDichVuGiuongBenhVienChiPhis
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhis ?? (_yeuCauDichVuGiuongBenhVienChiPhis = new List<YeuCauDichVuGiuongBenhVienChiPhi>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhis = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVienChiPhiBHYT> _yeuCauDichVuGiuongBenhVienChiPhiBHYTs;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhiBHYT> YeuCauDichVuGiuongBenhVienChiPhiBHYTs
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhiBHYTs ?? (_yeuCauDichVuGiuongBenhVienChiPhiBHYTs = new List<YeuCauDichVuGiuongBenhVienChiPhiBHYT>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhiBHYTs = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> _yeuCauDichVuGiuongBenhVienChiPhiBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> YeuCauDichVuGiuongBenhVienChiPhiBenhViens
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens ?? (_yeuCauDichVuGiuongBenhVienChiPhiBenhViens = new List<YeuCauDichVuGiuongBenhVienChiPhiBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens = value;
        }

        private ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs;
        public virtual ICollection<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong> ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs
        {
            get => _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs ?? (_chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs = new List<ChuongTrinhGoiDichVuKhuyenMaiDichVuGiuong>());
            protected set => _chuongTrinhGoiDichVuKhuyenMaiDichVuGiuongs = value;
        }

        private ICollection<NoiGioiThieuChiTietMienGiam> _noiGioiThieuChiTietMienGiams;
        public virtual ICollection<NoiGioiThieuChiTietMienGiam> NoiGioiThieuChiTietMienGiams
        {
            get => _noiGioiThieuChiTietMienGiams ?? (_noiGioiThieuChiTietMienGiams = new List<NoiGioiThieuChiTietMienGiam>());
            protected set => _noiGioiThieuChiTietMienGiams = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong> _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong> NoiGioiThieuHopDongChiTietHoaHongDichVuGiuongs
        {
            get => _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs ?? (_noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs = new List<NoiGioiThieuHopDongChiTietHoaHongDichVuGiuong>());
            protected set => _noiGioiThieuHopDongChiTietHoaHongDichVuGiuongs = value;
        }

        private ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong> _noiGioiThieuHopDongChiTietHeSoDichVuGiuongs;
        public virtual ICollection<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong> NoiGioiThieuHopDongChiTietHeSoDichVuGiuongs
        {
            get => _noiGioiThieuHopDongChiTietHeSoDichVuGiuongs ?? (_noiGioiThieuHopDongChiTietHeSoDichVuGiuongs = new List<NoiGioiThieuHopDongChiTietHeSoDichVuGiuong>());
            protected set => _noiGioiThieuHopDongChiTietHeSoDichVuGiuongs = value;
        }
    }
}
