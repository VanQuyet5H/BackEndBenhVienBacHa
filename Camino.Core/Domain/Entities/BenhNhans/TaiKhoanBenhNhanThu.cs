using System;
using System.Collections.Generic;
using Camino.Core.Domain.Entities.CongTyBaoHiemTuNhans;
using Camino.Core.Domain.Entities.DonThuocThanhToans;
using Camino.Core.Domain.Entities.DonVatTus;
using Camino.Core.Domain.Entities.MauVaChePhams;
using Camino.Core.Domain.Entities.NhanViens;
using Camino.Core.Domain.Entities.PhongBenhViens;
using Camino.Core.Domain.Entities.YeuCauKhamBenhs;
using Camino.Core.Domain.Entities.YeuCauTiepNhans;

namespace Camino.Core.Domain.Entities.BenhNhans
{
    public class TaiKhoanBenhNhanThu : BaseEntity
    {
        public long TaiKhoanBenhNhanId { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public Enums.LoaiThuTienBenhNhan LoaiThuTienBenhNhan { get; set; }
        public Enums.LoaiNoiThu LoaiNoiThu { get; set; }
        public decimal? TienMat { get; set; }
        public decimal? ChuyenKhoan { get; set; }
        public decimal? POS { get; set; }
        public decimal? CongNo { get; set; }
        public string NoiDungThu { get; set; }
        public DateTime NgayThu { get; set; }
        public int? SoQuyen { get; set; }
        public int? SoPhieu { get; set; }
        public long? HoanTraYeuCauKhamBenhId { get; set; }
        public long? HoanTraYeuCauDichVuKyThuatId { get; set; }
        public long? HoanTraYeuCauDuocPhamBenhVienId { get; set; }
        public long? HoanTraYeuCauVatTuBenhVienId { get; set; }
        public long? HoanTraYeuCauDichVuGiuongBenhVienId { get; set; }
        public long? HoanTraYeuCauGoiDichVuId { get; set; }
        public long? HoanTraDonThuocThanhToanId { get; set; }
        public long? HoanTraDonVTYTThanhToanId { get; set; }
        public long? HoanTraYeuCauDichVuGiuongBenhVienChiPhiBenhVienId { get; set; }
        public long? HoanTraYeuCauTruyenMauId { get; set; }
        public long NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public long? TaiKhoanBenhNhanHuyDichVuId { get; set; }

        public string SoPhieuHienThi { get; set; }
        public bool? DaHuy { get; set; }
        public long? NhanVienHuyId { get; set; }
        public DateTime? NgayHuy { get; set; }
        public long? NoiHuyId { get; set; }
        public string LyDoHuy { get; set; }
        public long? PhieuHoanUngId { get; set; }
        public decimal? TamUng { get; set; }
        public bool? DaThuNo { get; set; }
        public long? ThuNoPhieuThuId { get; set; }
        public bool? DaThuHoi { get; set; }
        public long? NhanVienThuHoiId { get; set; }
        public DateTime? NgayThuHoi { get; set; }
        public bool? ThuTienGoiDichVu { get; set; }

        //BVHD-3951
        public string GhiChu { get; set; }

        public virtual NhanVien NhanVienHuy { get; set; }
        public virtual NhanVien NhanVienThuHoi { get; set; }
        public virtual PhongBenhVien NoiHuy { get; set; }
        public virtual TaiKhoanBenhNhanChi PhieuHoanUng { get; set; }
        public virtual TaiKhoanBenhNhanThu ThuNoPhieuThu { get; set; }

        public virtual TaiKhoanBenhNhan TaiKhoanBenhNhan { get; set; }
        public virtual YeuCauTiepNhan YeuCauTiepNhan { get; set; }
        public virtual YeuCauKhamBenh HoanTraYeuCauKhamBenh { get; set; }
        public virtual YeuCauDichVuKyThuat HoanTraYeuCauDichVuKyThuat { get; set; }
        public virtual YeuCauDuocPhamBenhVien HoanTraYeuCauDuocPhamBenhVien { get; set; }
        public virtual YeuCauVatTuBenhVien HoanTraYeuCauVatTuBenhVien { get; set; }
        public virtual YeuCauDichVuGiuongBenhVien HoanTraYeuCauDichVuGiuongBenhVien { get; set; }
        public virtual YeuCauGoiDichVu HoanTraYeuCauGoiDichVu { get; set; }
        public virtual DonThuocThanhToan HoanTraDonThuocThanhToan { get; set; }
        public virtual DonVTYTThanhToan HoanTraDonVTYTThanhToan { get; set; }
        public virtual YeuCauDichVuGiuongBenhVienChiPhiBenhVien HoanTraYeuCauDichVuGiuongBenhVienChiPhiBenhVien { get; set; }
        public virtual YeuCauTruyenMau HoanTraYeuCauTruyenMau { get; set; }
        public virtual NhanVien NhanVienThucHien { get; set; }
        public virtual PhongBenhVien NoiThucHien { get; set; }
        public virtual TaiKhoanBenhNhanHuyDichVu TaiKhoanBenhNhanHuyDichVu { get; set; }

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

        private ICollection<TaiKhoanBenhNhanThu> _taiKhoanBenhNhanThus;
        public virtual ICollection<TaiKhoanBenhNhanThu> TaiKhoanBenhNhanThus
        {
            get => _taiKhoanBenhNhanThus ?? (_taiKhoanBenhNhanThus = new List<TaiKhoanBenhNhanThu>());
            protected set => _taiKhoanBenhNhanThus = value;
        }
    }
}
