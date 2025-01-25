using System;
using System.Collections.Generic;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.YeuCauTiepNhans;
using Camino.Core.Helpers;

namespace Camino.Api.Models.ThongTinBenhNhan
{
    public class ThongTinThuNganBenhNhanViewModel : BaseViewModel
    {
        ThongTinThuNganBenhNhanViewModel()
        {
            ThongTinBHYTNoiTrus = new List<ThongTinBHYTNoiTru>();
        }
        #region THÔNG TIN HÀNH CHÍNH
        public string MaYeuCauTiepNhan { get; set; }
        public long? BenhNhanId { get; set; }
        public string MaBN { get; set; }
        public string HoTen { get; set; }
        public string DiaChi { get; set; }
        public int? NamSinh { get; set; }
        public string NgayThangNamSinh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public long? DoiTuongUuDaiId { get; set; }
        public string DoiTuongUuDai { get; set; }
        public long? CongTyUuDaiId { get; set; }
        public string CongTyUuDai { get; set; }
        public Enums.LoaiGioiTinh? GioiTinh { get; set; }
        public string GioiTinhStr => GioiTinh.GetDescription();
        #endregion

        #region THÔNG TIN BHYT

        public bool CoBHYT { get; set; }
        public string BHYTMaSoThe { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public string NgayHieuLucStr => BHYTNgayHieuLuc?.ApplyFormatDate();
        public DateTime? BHYTNgayHetHan { get; set; }
        public string NgayHetHanStr => BHYTNgayHetHan?.ApplyFormatDate();
        public string BHYTDiaChi { get; set; }
        public string BHYTMucHuong { get; set; }
        public Enums.EnumLyDoVaoVien? LyDoVaoVien { get; set; }
        public string TuyenKham => LyDoVaoVien == null ? null : LyDoVaoVien.GetDescription();
        public long? GiayChuyenVienId { get; set; }
        public string TenGiayChuyenVien { get; set; }
        public string GiayChuyenVienUrl { get; set; }
        public long? BHYTgiayMienCungChiTraId { get; set; }
        public string TenBHYTgiayMienCungChiTra { get; set; }
        public string BHYTgiayMienCungChiTraUrl { get; set; }
        public List<ThongTinCongTyBaoHiemTuNhan> ThongTinCongTyBaoHiemTuNhans { get; set; }
        public TaiLieuDinhKemGiayChuyenVien TaiLieuDinhKemGiayChuyenVien { get; set; }
        public TaiLieuDinhKemGiayMiemCungChiTra TaiLieuDinhKemGiayMiemCungChiTra { get; set; }
        public TaiLieuDinhKemGiayMiemGiam TaiLieuDinhKemGiayMiemGiam { get; set; }

        #endregion

        public string SoBenhAn { get; set; }
        public string DoiTuong => CoBHYT == true ? "BHYT" : "Viện phí";
        public int? Tuoi => NamSinh != null ? (DateTime.Now.Year - NamSinh) : null;
        #region Cập nhật bảo hiểm y tế cho phần nội trú 23/02/2021
        public List<ThongTinBHYTNoiTru> ThongTinBHYTNoiTrus { get; set; }
        #endregion

        //BVHD-3800
        public bool? LaCapCuu { get; set; }
        public bool DangDieuTriNoiTru { get; set; }
        public bool CoNoiGioiThieu { get; set; }
        public string TenNoiGioiThieu { get; set; }

        //BVHD-3941
        public bool? CoYCTN { get; set; }
        public string TenCongTyBaoHiemTuNhan { get; set; }
    }

    #region THONG TIN BAO HIEM TU NHAN

    public class ThongTinBHYTNoiTru : BaseViewModel
    {
        public bool? CoBHYT { get; set; }
        public string BHYTMaSoThe { get; set; }
        public DateTime? BHYTNgayHieuLuc { get; set; }
        public string NgayHieuLucStr => BHYTNgayHieuLuc?.ApplyFormatDate();
        public DateTime? BHYTNgayHetHan { get; set; }
        public string NgayHetHanStr => BHYTNgayHetHan?.ApplyFormatDate();
        public string BHYTDiaChi { get; set; }
        public string BHYTMucHuong { get; set; }
        public TaiLieuDinhKemGiayMiemCungChiTra TaiLieuDinhKemGiayMiemCungChiTra { get; set; }
    }


    public class ThongTinCongTyBaoHiemTuNhan : BaseViewModel
    {
        public string TenCongTy { get; set; }
        public string MaSoThe { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public decimal SoTienCongNo { get; set; }
        public DateTime? NgayHieuLuc { get; set; }
        public string TuNgayStr => NgayHieuLuc?.ApplyFormatDate();
        public DateTime? NgayHetHan { get; set; }
        public string DenNgayStr => NgayHetHan?.ApplyFormatDate();
    }

    public class TaiLieuDinhKemGiayChuyenVien
    {
        public string UploadUrl { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
        public string DuongDanTmp { get; set; }
        public long KichThuoc { get; set; }
        public int LoaiTapTin { get; set; }
    }

    public class TaiLieuDinhKemGiayMiemCungChiTra
    {
        public string UploadUrl { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public string DuongDan { get; set; }
        public string DuongDanTmp { get; set; }
        public long KichThuoc { get; set; }
        public int LoaiTapTin { get; set; }
    }


    #endregion
}