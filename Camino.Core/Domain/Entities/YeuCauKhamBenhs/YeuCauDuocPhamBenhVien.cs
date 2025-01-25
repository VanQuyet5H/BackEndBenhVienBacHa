using Camino.Core.Domain.Entities.DuocPhamBenhViens;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DonViTinhs;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauLinhDuocPhams;
using Camino.Core.Domain.Entities.KhoDuocPhams;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using static Camino.Core.Domain.Enums;
using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauDuocPhamBenhVien : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }

        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long DuocPhamBenhVienId { get; set; }
        //public long XuatKhoDuocPhamChiTietViTriId { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? STTHoatChat { get; set; }
        //public int NhomChiPhi { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat LoaiThuocHoacHoatChat { get; set; }

        public decimal? DonGiaBaoHiem { get; set; }

        public int? MucHuongBaoHiem { get; set; }

        public int? TiLeBaoHiemThanhToan { get; set; }

        public string NhaSanXuat { get; set; }
        public string NuocSanXuat { get; set; }
        public long DuongDungId { get; set; }
        public string HamLuong { get; set; }
        public string QuyCach { get; set; }
        public string TieuChuan { get; set; }
        public string DangBaoChe { get; set; }
        public long DonViTinhId { get; set; }
        public string HuongDan { get; set; }
        public string MoTa { get; set; }
        public string ChiDinh { get; set; }
        public string ChongChiDinh { get; set; }
        public string LieuLuongCachDung { get; set; }
        public string TacDungPhu { get; set; }
        public string ChuYdePhong { get; set; }
        public long? HopDongThauDuocPhamId { get; set; }
        public long? NhaThauId { get; set; }
        public string SoHopDongThau { get; set; }
        public string SoQuyetDinhThau { get; set; }
        public Enums.EnumLoaiThau? LoaiThau { get; set; }
        public Enums.EnumLoaiThuocThau? LoaiThuocThau { get; set; }
        public string NhomThau { get; set; }
        public string GoiThau { get; set; }
        public int? NamThau { get; set; }
        public bool? KhongTinhPhi { get; set; }
        //public decimal? Gia { get; set; }
        //public long? GoiDichVuId { get; set; }
        //public int? TiLeChietKhau { get; set; }
        public double SoLuong { get; set; }
        public double? SoLuongDaTra { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public long NoiChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public long? NoiCapThuocId { get; set; }
        public long? NhanVienCapThuocId { get; set; }
        public DateTime? ThoiDiemCapThuoc { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        //public bool DaThanhToan { get; set; }
        //public long? NoiThanhToanId { get; set; }
        //public long? NhanVienThanhToanId { get; set; }
        //public DateTime? ThoiDiemThanhToan { get; set; }
        public bool DaCapThuoc { get; set; }
        public Enums.EnumYeuCauDuocPhamBenhVien TrangThai { get; set; }
        public string GhiChu { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }

        public long? NhanVienHuyThanhToanId { get; set; }

        public string LyDoHuyThanhToan { get; set; }

        public long? XuatKhoDuocPhamChiTietId { get; set; }

        public Enums.EnumLoaiPhieuLinh LoaiPhieuLinh { get; set; }

        public long? YeuCauLinhDuocPhamId { get; set; }

        public bool LaDuocPhamBHYT { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public PhuongPhapTinhGiaTriTonKho? PhuongPhapTinhGiaTriTonKho { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal DonGiaBan { get; private set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal GiaBan { get; private set; }
        public long? KhoLinhId { get; set; }
        public EnumGiaiDoanPhauThuat? GiaiDoanPhauThuat { get; set; }

        public double? SoLuongDaLinhBu { get; set; }
        public bool? KhongLinhBu { get; set; }

        public long? NoiTruPhieuDieuTriId { get; set; }
        //public int? SoLanDungTrongNgay { get; set; }
        //public double? DungSang { get; set; }
        //public double? DungTrua { get; set; }
        //public double? DungChieu { get; set; }
        //public double? DungToi { get; set; }
        //public int? ThoiGianDungSang { get; set; }
        //public int? ThoiGianDungTrua { get; set; }
        //public int? ThoiGianDungChieu { get; set; }
        //public int? ThoiGianDungToi { get; set; }
        public bool? LaDichTruyen { get; set; }
        //public int? TocDoTruyen { get; set; }
        //public DonViTocDoTruyen? DonViTocDoTruyen { get; set; }
        //public int? ThoiGianBatDauTruyen { get; set; }
        //public double? CachGioTruyenDich { get; set; }
        public int? TheTich { get; set; }
        public long? NoiTruChiDinhDuocPhamId { get; set; }
        public long? YeuCauTiepNhanTheBHYTId { get; set; }
        public string MaSoTheBHYT { get; set; }

        public long? NguoiDanhDauKhongBuId { get; set; }
        public DateTime? NgayDanhDauKhongBu { get; set; }

        public virtual YeuCauTiepNhanTheBHYT YeuCauTiepNhanTheBHYT { get; set; }
        public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }
        public virtual Kho KhoLinh { get; set; }
        public virtual XuatKhoDuocPhamChiTiet XuatKhoDuocPhamChiTiet { get; set; }

        public virtual YeuCauLinhDuocPham YeuCauLinhDuocPham { get; set; }

        public virtual NhanVien NhanVienHuyThanhToan { get; set; }

        public virtual DuocPhamBenhVien DuocPhamBenhVien { get; set; }
        //public virtual GoiDichVu GoiDichVu { get; set; }
        public virtual NhanVien NhanVienCapThuoc { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }
        //public virtual NhanVien NhanVienThanhToan { get; set; }
        public virtual PhongBenhVien NoiCapThuoc { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        //public virtual PhongBenhVien NoiThanhToan { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual NhaThau NhaThau { get; set; }
        public virtual HopDongThauDuocPham HopDongThauDuocPham { get; set; }
        public virtual DonViTinh DonViTinh { get; set; }
        public virtual DuongDung DuongDung { get; set; }
        public virtual YeuCauGoiDichVu YeuCauGoiDichVu { get; set; }
        public virtual NoiTruChiDinhDuocPham NoiTruChiDinhDuocPham { get; set; }

        //public virtual XuatKhoDuocPhamChiTietViTri XuatKhoDuocPhamChiTietViTri { get; set; }

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
        private ICollection<YeuCauLinhDuocPhamChiTiet> _yeuCauLinhDuocPhamChiTiets;
        public virtual ICollection<YeuCauLinhDuocPhamChiTiet> YeuCauLinhDuocPhamChiTiets
        {
            get => _yeuCauLinhDuocPhamChiTiets ?? (_yeuCauLinhDuocPhamChiTiets = new List<YeuCauLinhDuocPhamChiTiet>());
            protected set => _yeuCauLinhDuocPhamChiTiets = value;
        }
        
        #region clone
        public YeuCauDuocPhamBenhVien Clone()
		{
			return (YeuCauDuocPhamBenhVien)this.MemberwiseClone();
		}
        #endregion
        

        private ICollection<YeuCauTraDuocPhamTuBenhNhanChiTiet> _yeuCauTraDuocPhamTuBenhNhanChiTiets;
        public virtual ICollection<YeuCauTraDuocPhamTuBenhNhanChiTiet> YeuCauTraDuocPhamTuBenhNhanChiTiets
        {
            get => _yeuCauTraDuocPhamTuBenhNhanChiTiets ?? (_yeuCauTraDuocPhamTuBenhNhanChiTiets = new List<YeuCauTraDuocPhamTuBenhNhanChiTiet>());
            protected set => _yeuCauTraDuocPhamTuBenhNhanChiTiets = value;
        }
    }
}
