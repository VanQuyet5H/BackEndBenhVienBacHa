using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.BenhNhans;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DichVuGiuongBenhViens;
using Camino.Core.Domain.Entities.DuyetBaoHiems;
using Camino.Core.Domain.Entities.GiuongBenhs;
using Camino.Core.Domain.Entities.GoiDichVus;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.NoiDungGhiChuMiemGiams;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;
using Camino.Core.Domain.ValueObject.Grid;

namespace Camino.Core.Domain.Entities.YeuCauKhamBenhs
{
    public class YeuCauDichVuGiuongBenhVien : BaseEntity
    {
        public long YeuCauTiepNhanId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long DichVuGiuongBenhVienId { get; set; }
        public long? NhomGiaDichVuGiuongBenhVienId { get; set; }
        
        public string Ten { get; set; }
        public string Ma { get; set; }
        public string MaTT37 { get; set; }
        public Enums.EnumLoaiGiuong LoaiGiuong { get; set; }
        public string MoTa { get; set; }
        public bool? KhongTinhPhi { get; set; }
        public decimal? Gia { get; set; }
        public decimal? DonGiaTruocChietKhau { get; set; }
        public decimal? DonGiaSauChietKhau { get; set; }

        public long? YeuCauGoiDichVuId { get; set; }
        //public long? GoiDichVuId { get; set; }

        //public int? TiLeChietKhau { get; set; }

        public long NhanVienChiDinhId { get; set; }
        public long NoiChiDinhId { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }

        public decimal? DonGiaBaoHiem { get; set; }

        public int? MucHuongBaoHiem { get; set; }

        public DateTime ThoiDiemChiDinh { get; set; }
        public long? NoiThucHienId { get; set; }
        public string MaGiuong { get; set; }
        public string TenGiuong { get; set; }

        public DateTime? ThoiDiemBatDauSuDung { get; set; }
        public DateTime? ThoiDiemKetThucSuDung { get; set; }
        public bool DuocHuongBaoHiem { get; set; }
        public bool? BaoHiemChiTra { get; set; }
        public DateTime? ThoiDiemDuyetBaoHiem { get; set; }
        public long? NhanVienDuyetBaoHiemId { get; set; }
        //public long DaThanhToan { get; set; }
        //public long? NoiThanhToanId { get; set; }
        //public long? NhanVienThanhToanId { get; set; }
        //public DateTime? ThoiDiemThanhToan { get; set; }
        public Enums.EnumTrangThaiGiuongBenh TrangThai { get; set; }
        public string GhiChu { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public string GhiChuMienGiamThem { get; set; }
        public decimal? SoTienBenhNhanDaChi { get; set; }
        public Enums.TrangThaiThanhToan TrangThaiThanhToan { get; set; }

        public string LyDoHuyThanhToan { get; set; }

        public long? NhanVienHuyThanhToanId { get; set; }

        public long? GiuongBenhId { get; set; }
        public Enums.DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public bool? BaoPhong { get; set; }
        public virtual GiuongBenh GiuongBenh { get; set; }

        public virtual NhanVien NhanVienHuyThanhToan { get; set; }

        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual DichVuGiuongBenhVien DichVuGiuongBenhVien { get; set; }
        public virtual NhomGiaDichVuGiuongBenhVien NhomGiaDichVuGiuongBenhVien { get; set; }
        //public virtual GoiDichVu GoiDichVu { get; set; }
        public virtual NhanVien NhanVienChiDinh { get; set; }
        public virtual PhongBenhVien NoiChiDinh { get; set; }
        public virtual PhongBenhVien NoiThucHien { get; set; }
        //public virtual PhongBenhVien NoiThanhToan { get; set; }
        //public virtual NhanVien NhanVienThanhToan { get; set; }
        public virtual NhanVien NhanVienDuyetBaoHiem { get; set; }

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

        private ICollection<HoatDongGiuongBenh> _hoatDongGiuongBenhs;
        public virtual ICollection<HoatDongGiuongBenh> HoatDongGiuongBenhs
        {
            get => _hoatDongGiuongBenhs ?? (_hoatDongGiuongBenhs = new List<HoatDongGiuongBenh>());
            protected set => _hoatDongGiuongBenhs = value;
        }
    }
}
