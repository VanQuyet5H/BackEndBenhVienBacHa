using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DieuTriNoiTrus;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.Entities.YeuCauTiepNhanTheBHYTs;

namespace Camino.Core.Domain.Entities.MauVaChePhams
{
    public class YeuCauTruyenMau : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }
        public long MauVaChePhamId { get; set; }
        public string MaDichVu { get; set; }
        public string TenDichVu { get; set; }
        public Enums.PhanLoaiMau PhanLoaiMau { get; set; }
        public long TheTich { get; set; }
        public Enums.EnumNhomMau? NhomMau { get; set; }
        public Enums.EnumYeuToRh? YeuToRh { get; set; }
        public long? XuatKhoMauChiTietId { get; set; }
        public decimal? DonGiaNhap { get; set; }
        public decimal? DonGiaBan { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        public string MoTa { get; set; }
        public Enums.EnumTrangThaiYeuCauTruyenMau TrangThai { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }
        public long? NhanVienHuyThanhToanId { get; set; }
        public string LyDoHuyThanhToan { get; set; }
        public long NhanVienChiDinhId { get; set; }
        public long NoiChiDinhId { get; set; }
        public DateTime ThoiDiemChiDinh { get; set; }
        public int? ThoiGianBatDauTruyen { get; set; }
        public long? NoiThucHienId { get; set; }
        public string MaGiuong { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public DateTime? ThoiDiemHoanThanh { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public string GhiChuMienGiamThem { get; set; }

        //BVHD-3731
        public long? NoiDungGhiChuMiemGiamId { get; set; }
        public virtual NoiDungGhiChuMiemGiam NoiDungGhiChuMiemGiam { get; set; }

        public long? YeuCauTiepNhanTheBHYTId { get; set; }
        public string MaSoTheBHYT { get; set; }
        public DateTime? ThoiGianDienBien { get; set; }

        public virtual YeuCauTiepNhanTheBHYT YeuCauTiepNhanTheBHYT { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual NoiTruPhieuDieuTri NoiTruPhieuDieuTri { get; set; }
        public virtual MauVaChePham MauVaChePham { get; set; }
        public virtual XuatKhoMauChiTiet XuatKhoMauChiTiet { get; set; }
        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }
        public virtual NhanVien NhanVienHuyThanhToan { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        public virtual PhongBenhVien NoiThucHien { get; set; }
        public virtual NhanVien NhanVienThucHien { get; set; }

        private ICollection<NhapKhoMauChiTiet> _nhapKhoMauChiTiets;
        public virtual ICollection<NhapKhoMauChiTiet> NhapKhoMauChiTiets
        {
            get => _nhapKhoMauChiTiets ?? (_nhapKhoMauChiTiets = new List<NhapKhoMauChiTiet>());
            protected set => _nhapKhoMauChiTiets = value;
        }

        private ICollection<CongTyBaoHiemTuNhanCongNo> _baoHiemTuNhanCongNos;
        public virtual ICollection<CongTyBaoHiemTuNhanCongNo> CongTyBaoHiemTuNhanCongNos
        {
            get => _baoHiemTuNhanCongNos ?? (_baoHiemTuNhanCongNos = new List<CongTyBaoHiemTuNhanCongNo>());
            protected set => _baoHiemTuNhanCongNos = value;
        }

        private ICollection<DuyetBaoHiemChiTiet> _duyetBaoHiemChiTiets;
        public virtual ICollection<DuyetBaoHiemChiTiet> DuyetBaoHiemChiTiets
        {
            get => _duyetBaoHiemChiTiets ?? (_duyetBaoHiemChiTiets = new List<DuyetBaoHiemChiTiet>());
            protected set => _duyetBaoHiemChiTiets = value;
        }

        private ICollection<MienGiamChiPhi> _mienGiamChiPhis;
        public virtual ICollection<MienGiamChiPhi> MienGiamChiPhis
        {
            get => _mienGiamChiPhis ?? (_mienGiamChiPhis = new List<MienGiamChiPhi>());
            protected set => _mienGiamChiPhis = value;
        }

        private ICollection<NoiTruPhieuDieuTriChiTietYLenh> _noiTruPhieuDieuTriChiTietYLenhs;
        public virtual ICollection<NoiTruPhieuDieuTriChiTietYLenh> NoiTruPhieuDieuTriChiTietYLenhs
        {
            get => _noiTruPhieuDieuTriChiTietYLenhs ?? (_noiTruPhieuDieuTriChiTietYLenhs = new List<NoiTruPhieuDieuTriChiTietYLenh>());
            protected set => _noiTruPhieuDieuTriChiTietYLenhs = value;
        }

        private ICollection<TaiKhoanBenhNhanChi> _taiKhoanBenhNhanChis;
        public virtual ICollection<TaiKhoanBenhNhanChi> TaiKhoanBenhNhanChis
        {
            get => _taiKhoanBenhNhanChis ?? (_taiKhoanBenhNhanChis = new List<TaiKhoanBenhNhanChi>());
            protected set => _taiKhoanBenhNhanChis = value;
        }

        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThus;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThus
        {
            get => _taiKhoanBenhNhanThus ?? (_taiKhoanBenhNhanThus = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThus = value;
        }
    }
}
