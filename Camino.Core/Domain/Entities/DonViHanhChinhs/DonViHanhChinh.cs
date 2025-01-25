using Camino.Core.Domain.Entities;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.KhamDoans;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.DonViHanhChinhs
{
    public class DonViHanhChinh : BaseEntity
    {
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long? TrucThuocDonViHanhChinhId { get; set; }
        public Enums.CapHanhChinh CapHanhChinh { get; set; }
        public string TenDonViHanhChinh { get; set; }
        public Enums.VungDiaLy? VungDiaLy { get; set; }
        public string TenVietTat { get; set; }

        public virtual DonViHanhChinh TrucThuocDonViHanhChinh { get; set; }

        private ICollection<DonViHanhChinh> _trucThuocDonViHanhChinhs;
        public virtual ICollection<DonViHanhChinh> TrucThuocDonViHanhChinhs
        {
            get => _trucThuocDonViHanhChinhs ?? (_trucThuocDonViHanhChinhs = new List<DonViHanhChinh>());
            protected set => _trucThuocDonViHanhChinhs = value;
        }

        private ICollection<BenhVien.BenhVien> _benhViens;
        public virtual ICollection<BenhVien.BenhVien> BenhViens
        {
            get => _benhViens ?? (_benhViens = new List<BenhVien.BenhVien>());
            protected set => _benhViens = value;
        }
        private ICollection<BenhNhan> _phuongXaBenhNhans;
        public virtual ICollection<BenhNhan> PhuongXaBenhNhans
        {
            get => _phuongXaBenhNhans ?? (_phuongXaBenhNhans = new List<BenhNhan>());
            protected set => _phuongXaBenhNhans = value;
        }
        private ICollection<BenhNhan> _quanHuyenBenhNhans;
        public virtual ICollection<BenhNhan> QuanHuyenBenhNhans
        {
            get => _quanHuyenBenhNhans ?? (_quanHuyenBenhNhans = new List<BenhNhan>());
            protected set => _quanHuyenBenhNhans = value;
        }
        private ICollection<BenhNhan> _tinhThanhBenhNhans;
        public virtual ICollection<BenhNhan> TinhThanhBenhNhans
        {
            get => _tinhThanhBenhNhans ?? (_tinhThanhBenhNhans = new List<BenhNhan>());
            protected set => _tinhThanhBenhNhans = value;
        }

        #region Update 12/2/2020

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanPhuongXas;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanPhuongXas
        {
            get => _yeuCauTiepNhanPhuongXas ?? (_yeuCauTiepNhanPhuongXas = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanPhuongXas = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanQuanHuyens;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanQuanHuyens
        {
            get => _yeuCauTiepNhanQuanHuyens ?? (_yeuCauTiepNhanQuanHuyens = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanQuanHuyens = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanTinhThanhs;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanTinhThanhs
        {
            get => _yeuCauTiepNhanTinhThanhs ?? (_yeuCauTiepNhanTinhThanhs = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanTinhThanhs = value;
        }

        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNguoiLienHeTinhThanhs;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNguoiLienHeTinhThanhs
        {
            get => _yeuCauTiepNhanNguoiLienHeTinhThanhs ?? (_yeuCauTiepNhanNguoiLienHeTinhThanhs = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNguoiLienHeTinhThanhs = value;
        }
        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNguoiLienHeQuanHuyens;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNguoiLienHeQuanHuyens
        {
            get => _yeuCauTiepNhanNguoiLienHeQuanHuyens ?? (_yeuCauTiepNhanNguoiLienHeQuanHuyens = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNguoiLienHeQuanHuyens = value;
        }
        private ICollection<YeuCauTiepNhan> _yeuCauTiepNhanNguoiLienHePhuongXas;
        public virtual ICollection<YeuCauTiepNhan> YeuCauTiepNhanNguoiLienHePhuongXas
        {
            get => _yeuCauTiepNhanNguoiLienHePhuongXas ?? (_yeuCauTiepNhanNguoiLienHePhuongXas = new List<YeuCauTiepNhan>());
            protected set => _yeuCauTiepNhanNguoiLienHePhuongXas = value;
        }

        #endregion Update 12/2/2020

        private ICollection<BenhNhan> _nguoiLienHePhuongXaBenhNhans;
        public virtual ICollection<BenhNhan> NguoiLienHePhuongXaBenhNhans
        {
            get => _nguoiLienHePhuongXaBenhNhans ?? (_nguoiLienHePhuongXaBenhNhans = new List<BenhNhan>());
            protected set => _nguoiLienHePhuongXaBenhNhans = value;
        }

        private ICollection<BenhNhan> _nguoiLienHeQuanHuyenBenhNhans;
        public virtual ICollection<BenhNhan> NguoiLienHeQuanHuyenBenhNhans
        {
            get => _nguoiLienHeQuanHuyenBenhNhans ?? (_nguoiLienHeQuanHuyenBenhNhans = new List<BenhNhan>());
            protected set => _nguoiLienHeQuanHuyenBenhNhans = value;
        }

        private ICollection<BenhNhan> _nguoiLienHeTinhThanhBenhNhans;
        public virtual ICollection<BenhNhan> NguoiLienHeTinhThanhBenhNhans
        {
            get => _nguoiLienHeTinhThanhBenhNhans ?? (_nguoiLienHeTinhThanhBenhNhans = new List<BenhNhan>());
            protected set => _nguoiLienHeTinhThanhBenhNhans = value;
        }

        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoePhuongXaNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoePhuongXaNhanViens
        {
            get => _hopDongKhamSucKhoePhuongXaNhanViens ?? (_hopDongKhamSucKhoePhuongXaNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoePhuongXaNhanViens = value;
        }
        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeQuanHuyenNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeQuanHuyenNhanViens
        {
            get => _hopDongKhamSucKhoeQuanHuyenNhanViens ?? (_hopDongKhamSucKhoeQuanHuyenNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeQuanHuyenNhanViens = value;
        }
        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeTinhThanhNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeTinhThanhNhanViens
        {
            get => _hopDongKhamSucKhoeTinhThanhNhanViens ?? (_hopDongKhamSucKhoeTinhThanhNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeTinhThanhNhanViens = value;
        }

        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeHoKhauPhuongXaNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeHoKhauPhuongXaNhanViens
        {
            get => _hopDongKhamSucKhoeHoKhauPhuongXaNhanViens ?? (_hopDongKhamSucKhoeHoKhauPhuongXaNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeHoKhauPhuongXaNhanViens = value;
        }
        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeHoKhauQuanHuyenNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeHoKhauQuanHuyenNhanViens
        {
            get => _hopDongKhamSucKhoeHoKhauQuanHuyenNhanViens ?? (_hopDongKhamSucKhoeHoKhauQuanHuyenNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeHoKhauQuanHuyenNhanViens = value;
        }
        private ICollection<HopDongKhamSucKhoeNhanVien> _hopDongKhamSucKhoeHoKhauTinhThanhNhanViens;
        public virtual ICollection<HopDongKhamSucKhoeNhanVien> HopDongKhamSucKhoeHoKhauTinhThanhNhanViens
        {
            get => _hopDongKhamSucKhoeHoKhauTinhThanhNhanViens ?? (_hopDongKhamSucKhoeHoKhauTinhThanhNhanViens = new List<HopDongKhamSucKhoeNhanVien>());
            protected set => _hopDongKhamSucKhoeHoKhauTinhThanhNhanViens = value;
        }
    }
}
