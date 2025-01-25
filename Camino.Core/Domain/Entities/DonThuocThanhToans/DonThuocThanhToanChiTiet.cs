using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DonViTinhs;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.HopDongThauDuocPhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NhaThaus;
using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;
using Camino.Core.Domain.Entities.NoiTruDonThuocs;
using Camino.Core.Domain.Entities.Thuocs;
using Camino.Core.Domain.Entities.XuatKhos;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;
using static Camino.Core.Domain.Enums;

namespace Camino.Core.Domain.Entities.DonThuocThanhToans
{
    public class DonThuocThanhToanChiTiet : BaseEntity
    {
        public long DonThuocThanhToanId { get; set; }
        public long? YeuCauKhamBenhDonThuocChiTietId { get; set; }
        public long DuocPhamId { get; set; }
        public long XuatKhoDuocPhamChiTietViTriId { get; set; }
        public string Ten { get; set; }
        public string TenTiengAnh { get; set; }
        public string SoDangKy { get; set; }
        public int? STTHoatChat { get; set; }
        public Enums.EnumDanhMucNhomTheoChiPhi NhomChiPhi { get; set; }
        public string MaHoatChat { get; set; }
        public string HoatChat { get; set; }
        public Enums.LoaiThuocHoacHoatChat LoaiThuocHoacHoatChat { get; set; }
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

        public decimal? DonGiaBaoHiem { get; set; }

        public int? MucHuongBaoHiem { get; set; }

        public int? TiLeBaoHiemThanhToan { get; set; }

        public string ChongChiDinh { get; set; }
        public string LieuLuongCachDung { get; set; }
        public string TacDungPhu { get; set; }
        public string ChuYDePhong { get; set; }
        public long? HopDongThauDuocPhamId { get; set; }
        public long? NhaThauId { get; set; }
        public string SoHopDongThau { get; set; }
        public string SoQuyetDinhThau { get; set; }
        public Enums.EnumLoaiThau? LoaiThau { get; set; }
        public Enums.EnumLoaiThuocThau? LoaiThuocThau { get; set; }
        public string NhomThau { get; set; }
        public string GoiThau { get; set; }
        public int NamThau { get; set; }
        public decimal DonGiaNhap { get; set; }
        public int TiLeTheoThapGia { get; set; }
        public int VAT { get; set; }
        public PhuongPhapTinhGiaTriTonKho? PhuongPhapTinhGiaTriTonKho { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal DonGiaBan { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal GiaBan { get; set; }
        public double SoLuong { get; set; }
        public int? SoNgayDung { get; set; }
        public double? DungSang { get; set; }
        public double? DungTrua { get; set; }
        public double? DungChieu { get; set; }
        public double? DungToi { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        //public decimal? GiaBaoHiemThanhToan { get; set; }
        public string GhiChu { get; set; }
        public long? YeuCauTiepNhanTheBHYTId { get; set; }
        public string MaSoTheBHYT { get; set; }
        public long? NoiTruDonThuocChiTietId { get; set; }

        public virtual YeuCauTiepNhanTheBHYT YeuCauTiepNhanTheBHYT { get; set; }
        public virtual DonThuocThanhToan DonThuocThanhToan { get; set; }
        public virtual YeuCauKhamBenhDonThuocChiTiet YeuCauKhamBenhDonThuocChiTiet { get; set; }
        public virtual DuocPham DuocPham { get; set; }
        public virtual XuatKhoDuocPhamChiTietViTri XuatKhoDuocPhamChiTietViTri { get; set; }
        public virtual DuongDung DuongDung { get; set; }
        public virtual DonViTinh DonViTinh { get; set; }
        public virtual HopDongThauDuocPham HopDongThauDuocPham { get; set; }
        public virtual NhaThau NhaThau { get; set; }
        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }
        public virtual NoiTruDonThuocChiTiet NoiTruDonThuocChiTiet { get; set; }

        //BVHD-3731
        public long? NoiDungGhiChuMiemGiamId { get; set; }
        public virtual NoiDungGhiChuMiemGiam NoiDungGhiChuMiemGiam { get; set; }

        private ICollection<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTiets;
        public virtual ICollection<DuyetBaoHiemChiTiet> DuyetBaoHiemChiTiets
        {
            get => _duyetBaoHiemChiTiets ?? (_duyetBaoHiemChiTiets = new List<DuyetBaoHiemChiTiet>());
            protected set => _duyetBaoHiemChiTiets = value;
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
        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChis;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChis
        {
            get => _taiKhoanBenhNhanChis ?? (_taiKhoanBenhNhanChis = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChis = value;
        }
    }
}
