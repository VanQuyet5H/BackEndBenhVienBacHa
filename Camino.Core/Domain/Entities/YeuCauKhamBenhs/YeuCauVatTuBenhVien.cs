using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.VatTuBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.HopDongThauVatTus;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.Entities.NhomVatTus;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.Entities.XuatKhoVatTus;
using Camino.Core.Domain.Entities.YeuCauLinhVatTus;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauVatTuBenhVien : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long VatTuBenhVienId { get; set; }
        public string Ten { get; set; }
        public string Ma { get; set; }
        public long NhomVatTuId { get; set; }
        public string DonViTinh { get; set; }
        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string MoTa { get; set; }
        public bool? KhongTinhPhi { get; set; }
        //public decimal? Gia { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }

        public decimal? DonGiaBaoHiem { get; set; }

        public int? MucHuongBaoHiem { get; set; }

        public int? TiLeBaoHiemThanhToan { get; set; }

        public double SoLuong { get; set; }
        public double? SoLuongDaTra { get; set; }
        public long? HopDongThauVatTuId { get; set; }
        public long? NhaThauId { get; set; }
        public string SoHopDongThau { get; set; }
        public string SoQuyetDinhThau { get; set; }
        public Enums.EnumLoaiThau? LoaiThau { get; set; }
        public string NhomThau { get; set; }
        public string GoiThau { get; set; }
        public int NamThau { get; set; }

        public long NhanVienChiDinhId { get; set; }
        public long NoiChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public long? NoiCapVatTuId { get; set; }
        public long? NhanVienCapVatTuId { get; set; }
        public DateTime? ThoiDiemCapVatTu { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public long? NhanVienHuyThanhToanId { get; set; }
        public string LyDoHuyThanhToan { get; set; }
        public bool DaCapVatTu { get; set; }
        public Enums.EnumYeuCauVatTuBenhVien TrangThai { get; set; }
        public string GhiChu { get; set; }

        public long? XuatKhoVatTuChiTietId { get; set; }

        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }

        public long? YeuCauLinhVatTuId { get; set; }

        public bool LaVatTuBHYT { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal DonGiaBan { get; private set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal GiaBan { get; private set; }
        public long? KhoLinhId { get; set; }
        public double? SoLuongDaLinhBu { get; set; }
        public bool? KhongLinhBu { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public long? YeuCauTiepNhanTheBHYTId { get; set; }
        public string MaSoTheBHYT { get; set; }

        public long? NguoiDanhDauKhongBuId { get; set; }
        public DateTime? NgayDanhDauKhongBu { get; set; }

        public LoaiNoiChiDinh? LoaiNoiChiDinh { get; set; } //nhánh thach/BVHD-3247
        public virtual YeuCauTiepNhanTheBHYT YeuCauTiepNhanTheBHYT { get; set; }
        public EnumGiaiDoanPhauThuat? GiaiDoanPhauThuat { get; set; }
        public virtual Kho KhoLinh { get; set; }
        public virtual XuatKhoVatTuChiTiet XuatKhoVatTuChiTiet { get; set; }
        public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }

        public virtual YeuCauLinhVatTu YeuCauLinhVatTu { get; set; }
        public virtual HopDongThauVatTu HopDongThauVatTu { get; set; }
        public virtual NhaThau NhaThau { get; set; }
        public virtual NhanVien NhanVienCapVatTu { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
        public virtual NhanVien NhanVienThanhToan { get; set; }
        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }
        public virtual PhongBenhVien NoiCapVatTu { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        public virtual VatTuBenhVien VatTuBenhVien { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual NhomVatTu NhomVatTu { get; set; }
        public virtual YeuCauGoiDichVu YeuCauGoiDichVu { get; set; }

        //BVHD-3731
        public long? NoiDungGhiChuMiemGiamId { get; set; }
        public virtual NoiDungGhiChuMiemGiam NoiDungGhiChuMiemGiam { get; set; }

        private ICollection<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTiets;
        public virtual ICollection<DuyetBaoHiemChiTiet> DuyetBaoHiemChiTiets
        {
            get => _duyetBaoHiemChiTiets ?? (_duyetBaoHiemChiTiets = new List<DuyetBaoHiemChiTiet>());
            protected set => _duyetBaoHiemChiTiets = value;
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

        /// <summary>
        /// Update 14/10/2020
        /// </summary>
        /// <returns></returns>
        private ICollection<YeuCauLinhVatTuChiTiet> _yeuCauLinhVatTuChiTiets;
        public virtual ICollection<YeuCauLinhVatTuChiTiet> YeuCauLinhVatTuChiTiets
        {
            get => _yeuCauLinhVatTuChiTiets ?? (_yeuCauLinhVatTuChiTiets = new List<YeuCauLinhVatTuChiTiet>());
            protected set => _yeuCauLinhVatTuChiTiets = value;
        }
        #region clone
        public YeuCauVatTuBenhVien Clone()
		{
			return (YeuCauVatTuBenhVien)this.MemberwiseClone();
		}
        #endregion

        private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _noiTruPhieuDieuTriChiTietYLenhs;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NoiTruPhieuDieuTriChiTietYLenhs
        {
            get => _noiTruPhieuDieuTriChiTietYLenhs ?? (_noiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
            protected set => _noiTruPhieuDieuTriChiTietYLenhs = value;
        }

        private ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> _yeuCauTraVatTuTuBenhNhanChiTiets;
        public virtual ICollection<YeuCauTraVatTuTuBenhNhanChiTiet> YeuCauTraVatTuTuBenhNhanChiTiets
        {
            get => _yeuCauTraVatTuTuBenhNhanChiTiets ?? (_yeuCauTraVatTuTuBenhNhanChiTiets = new List<YeuCauTraVatTuTuBenhNhanChiTiet>());
            protected set => _yeuCauTraVatTuTuBenhNhanChiTiets = value;
        }
    }
}
