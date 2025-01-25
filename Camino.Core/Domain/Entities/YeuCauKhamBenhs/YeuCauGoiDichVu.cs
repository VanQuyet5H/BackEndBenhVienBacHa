using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using static Camino.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.ChuongTrinhGoiDichVus;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.XuatKhoQuaTangs;
using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauGoiDichVu : BaseEntity
    {
        //update goi dv 10/21
        //public long YeuCauTiepNhanId { get; set; }
        //public long? YeuCauKhamBenhId { get; set; }
        //public long GoiDichVuId { get; set; }
        //public EnumLoaiGoiDichVu LoaiGoiDichVu { get; set; }
        //public string Ten { get; set; }
        //public bool CoChietKhau { get; set; }
        //public long ChiPhiGoiDichVu { get; set; }
        //public string MoTa { get; set; }
        public long BenhNhanId { get; set; }
        public long? BenhNhanSoSinhId { get; set; }
        public long ChuongTrinhGoiDichVuId { get; set; }
        public string MaChuongTrinh { get; set; }
        public string TenChuongTrinh { get; set; }
        public decimal GiaTruocChietKhau { get; set; }
        public decimal GiaSauChietKhau { get; set; }
        public string TenGoiDichVu { get; set; }
        public string MoTaGoiDichVu { get; set; }
        public bool? GoiSoSinh { get; set; }
        public bool? BoPhanMarketingDangKy { get; set; }
        public bool? BoPhanMarketingDaNhanTien { get; set; }
        public bool? DaTangQua { get; set; }

        public long NhanVienChiDinhId { get; set; }
        public long? NhanVienTuVanId { get; set; }
        public long NoiChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public Domain.Enums.EnumTrangThaiYeuCauGoiDichVu TrangThai { get; set; }
        public bool? DaQuyetToan { get; set; }
        public DateTime? ThoiDiemQuyetToan { get; set; }
        public decimal? SoTienTraLai { get; set; }
        public long? NhanVienQuyetToanId { get; set; }
        public long? NoiQuyetToanId { get; set; }
        public string GhiChu { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public DateTime? ThoiDiemHuyQuyetToan { get; set; }
        public long? NhanVienHuyQuyetToanId { get; set; }
        public string LyDoHuyQuyetToan { get; set; }
        public bool? NgungSuDung { get; set; }

        //public virtual GoiDichVu GoiDichVu { get; set; }
        public virtual BenhNhan BenhNhan { get; set; }
        public virtual BenhNhan BenhNhanSoSinh { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
        public virtual NhanVien NhanVienTuVan { get; set; }
        public virtual NhanVien NhanVienQuyetToan { get; set; }
        public virtual NhanVien NhanVienHuyQuyetToan { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        public virtual PhongBenhVien NoiQuyetToan { get; set; }
        public virtual ChuongTrinhGoiDichVu ChuongTrinhGoiDichVu { get; set; }

        //BVHD-3731
        public long? NoiDungGhiChuMiemGiamId { get; set; }
        public virtual NoiDungGhiChuMiemGiam NoiDungGhiChuMiemGiam { get; set; }

        //public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        //public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }

        private ICollection<YeuCauDuocPhamBenhVien> _yeuCauDuocPhamBenhViens;
        public virtual ICollection<YeuCauDuocPhamBenhVien> YeuCauDuocPhamBenhViens
        {
            get => _yeuCauDuocPhamBenhViens ?? (_yeuCauDuocPhamBenhViens = new List<YeuCauDuocPhamBenhVien>());
            protected set => _yeuCauDuocPhamBenhViens = value;
        }

        private ICollection<YeuCauVatTuBenhVien> _yeuCauVatTuBenhViens;
        public virtual ICollection<YeuCauVatTuBenhVien> YeuCauVatTuBenhViens
        {
            get => _yeuCauVatTuBenhViens ?? (_yeuCauVatTuBenhViens = new List<YeuCauVatTuBenhVien>());
            protected set => _yeuCauVatTuBenhViens = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVien> _yeuCauDichVuGiuongBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVien> YeuCauDichVuGiuongBenhViens
        {
            get => _yeuCauDichVuGiuongBenhViens ?? (_yeuCauDichVuGiuongBenhViens = new List<YeuCauDichVuGiuongBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhViens = value;
        }

        private ICollection<YeuCauDichVuKyThuat> _yeuCauDichVuKyThuats;
        public virtual ICollection<YeuCauDichVuKyThuat> YeuCauDichVuKyThuats
        {
            get => _yeuCauDichVuKyThuats ?? (_yeuCauDichVuKyThuats = new List<YeuCauDichVuKyThuat>());
            protected set => _yeuCauDichVuKyThuats = value;
        }

        private ICollection<YeuCauKhamBenh> _yeuCauKhamBenhs;
        public virtual ICollection<YeuCauKhamBenh> YeuCauKhamBenhs
        {
            get => _yeuCauKhamBenhs ?? (_yeuCauKhamBenhs = new List<YeuCauKhamBenh>());
            protected set => _yeuCauKhamBenhs = value;
        }
        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThus;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThus
        {
            get => _taiKhoanBenhNhanThus ?? (_taiKhoanBenhNhanThus = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThus = value;
        }
        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChis;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChis
        {
            get => _taiKhoanBenhNhanChis ?? (_taiKhoanBenhNhanChis = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChis = value;
        }
        private ICollection<CongTyBaoHiemTuNhanCongNo> _baoHiemTuNhanCongNos;
        public virtual ICollection<CongTyBaoHiemTuNhanCongNo> CongTyBaoHiemTuNhanCongNos
        {
            get => _baoHiemTuNhanCongNos ?? (_baoHiemTuNhanCongNos = new List<CongTyBaoHiemTuNhanCongNo>());
            protected set => _baoHiemTuNhanCongNos = value;
        }

        private ICollection<MienGiamChiPhi> _mienGiamChiPhis;
        public virtual ICollection<MienGiamChiPhi> MienGiamChiPhis
        {
            get => _mienGiamChiPhis ?? (_mienGiamChiPhis = new List<MienGiamChiPhi>());
            protected set => _mienGiamChiPhis = value;
        }

        private ICollection<MienGiamChiPhi> _mienGiamChiPhiKhuyenMais;
        public virtual ICollection<MienGiamChiPhi> MienGiamChiPhiKhuyenMais
        {
            get => _mienGiamChiPhiKhuyenMais ?? (_mienGiamChiPhiKhuyenMais = new List<MienGiamChiPhi>());
            protected set => _mienGiamChiPhiKhuyenMais = value;
        }

        private ICollection<XuatKhoQuaTang> _xuatKhoQuaTangs;
        public virtual ICollection<XuatKhoQuaTang> XuatKhoQuaTangs
        {
            get => _xuatKhoQuaTangs ?? (_xuatKhoQuaTangs = new List<XuatKhoQuaTang>());
            protected set => _xuatKhoQuaTangs = value;
        }

        private ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> _yeuCauDichVuGiuongBenhVienChiPhiBenhViens;
        public virtual ICollection<YeuCauDichVuGiuongBenhVienChiPhiBenhVien> YeuCauDichVuGiuongBenhVienChiPhiBenhViens
        {
            get => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens ?? (_yeuCauDichVuGiuongBenhVienChiPhiBenhViens = new List<YeuCauDichVuGiuongBenhVienChiPhiBenhVien>());
            protected set => _yeuCauDichVuGiuongBenhVienChiPhiBenhViens = value;
        }
    }
}
