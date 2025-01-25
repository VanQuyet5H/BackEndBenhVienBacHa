using System;
using System.Collections.Generic;
using System.Text;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.BenhNhans
{
    public class TaiKhoanBenhNhanChi : BaseEntity
    {
        public long TaiKhoanBenhNhanId { get; set; }
        public Enums.LoaiChiTienBenhNhan LoaiChiTienBenhNhan { get; set; }
        public decimal? TienChiPhi { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public string NoiDungChi { get; set; }
        public DateTime NgayChi { get; set; }
        public int? SoQuyen { get; set; }
        public int? SoPhieu { get; set; }
        public long? TaiKhoanBenhNhanThuId { get; set; }
        public long? YeuCauKhamBenhId { get; set; }
        public long? YeuCauDichVuKyThuatId { get; set; }
        public long? YeuCauDuocPhamBenhVienId { get; set; }
        public long? YeuCauVatTuBenhVienId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienId { get; set; }
        public long? YeuCauGoiDichVuId { get; set; }
        public long? DonThuocThanhToanChiTietId { get; set; }
        public long? DonVTYTThanhToanChiTietId { get; set; }
        public long? YeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public long? YeuCauTruyenMauId { get; set; }
        public long? YeuCauTiepNhanId { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? DonThuocThanhToanChiTietTheoPhieuThuId { get; set; }
        public long? DonVTYTThanhToanChiTietTheoPhieuThuId { get; set; }

        public string SoPhieuHienThi { get; set; }
        public decimal? Gia { get; set; }
        public double? SoLuong { get; set; }
        public decimal? DonGiaBaoHiem { get; set; }
        public int? MucHuongBaoHiem { get; set; }
        public int? TiLeBaoHiemThanhToan { get; set; }
        public decimal? SoTienBaoHiemTuNhanChiTra { get; set; }
        public decimal? SoTienMienGiam { get; set; }
        public bool? DaHuy { get; set; }
        public DateTime? NgayHuy { get; set; }
        public long? NhanVienHuyId { get; set; }
        public long? NoiHuyId { get; set; }
        public string LyDoHuy { get; set; }
        public long? PhieuThanhToanChiPhiId { get; set; }
        public bool? DaThuHoi { get; set; }
        public long? NhanVienThuHoiId { get; set; }
        public DateTime? NgayThuHoi { get; set; }

        public virtual NhanVien NhanVienHuy { get; set; }
        public virtual NhanVien NhanVienThuHoi { get; set; }
        public virtual TaiKhoanBenhNhanChi PhieuThanhToanChiPhi { get; set; }

        public virtual TaiKhoanBenhNhan TaiKhoanBenhNhan { get; set; }
        public virtual TaiKhoanBenhNhanThu TaiKhoanBenhNhanThu { get; set; }
        public virtual YeuCauKhamBenh YeuCauKhamBenh { get; set; }
        public virtual YeuCauDichVuKyThuat YeuCauDichVuKyThuat { get; set; }
        public virtual YeuCauDuocPhamBenhVien YeuCauDuocPhamBenhVien { get; set; }
        public virtual YeuCauVatTuBenhVien YeuCauVatTuBenhVien { get; set; }
        public virtual YeuCauDichVuGiuongBenhVien YeuCauDichVuGiuongBenhVien { get; set; }
        public virtual YeuCauGoiDichVu YeuCauGoiDichVu { get; set; }
        public virtual DonThuocThanhToanChiTiet DonThuocThanhToanChiTiet { get; set; }
        public virtual DonVTYTThanhToanChiTiet DonVTYTThanhToanChiTiet { get; set; }
        public virtual YeuCauDichVuGiuongBenhVienChiPhiBenhVien YeuCauDichVuGiuongBenhVienChiPhiBenhVien { get; set; }
        public virtual YeuCauTruyenMau YeuCauTruyenMau { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual NhanVien NhanVienThucHien { get; set; }
        public virtual PhongBenhVien NoiThucHien { get; set; }
        public virtual PhongBenhVien NoiHuy { get; set; }
        public virtual DonThuocThanhToanChiTietTheoPhieuThu DonThuocThanhToanChiTietTheoPhieuThu { get; set; }
        public virtual DonVTYTThanhToanChiTietTheoPhieuThu DonVTYTThanhToanChiTietTheoPhieuThu { get; set; }

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

        public virtual TaiKhoanBenhNhanChiThongTin TaiKhoanBenhNhanChiThongTin { get; set; }
    }
}
