using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class GiayChungSinhViewModel : BaseViewModel
    {
        public GiayChungSinhViewModel()
        {
            HoSoKhacTreSoSinhs = new List<HoSoKhacTreSoSinhViewModel>();
        }

        public long YeuCauTiepNhanId { get; set; }
        public LoaiHoSoDieuTriNoiTru? LoaiHoSoDieuTriNoiTru { get; set; }
        public string HoTenCha { get; set; }
        public string GhiChu { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public string ThoiDiemThucHienDisplay { get; set; }
        public long? NhanVienDoDeId { get; set; }
        public string TenNhanVienDoDe { get; set; }
        public long? NhanVienGhiPhieuId { get; set; }
        public string TenNhanVienGhiPhieu { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? GiamDocChuyenMonId { get; set; }
        public string TenGiamDocChuyenMon { get; set; }
        public long? NoiThucHienId { get; set; }
        public string ThongTinHoSo { get; set; }
        public string So { get; set; }
        public string QuyenSo { get; set; }
        public List<HoSoKhacTreSoSinhViewModel> HoSoKhacTreSoSinhs { get; set; }
    }
    public class HoSoKhacTreSoSinhViewModel
    {
        public long? YeuCauTiepNhanId { get; set; }
        public string GioiTinh { get; set; }
        public string HoTenCon { get; set; }
        public string CanNang { get; set; }
        public string GhiChu { get; set; }

    }

    #region BVHD-3705
    public class GiayChungSinhNewVo
    {
        public GiayChungSinhNewVo()
        {
            FileDinhKems = new List<FileDinhKemViewModel>();
        }
        public long NoiTruHoSoKhacId  { get; set; }
        public string So { get; set; }
        public string QuyenSo { get; set; }
        public string HoVaTenCha { get; set; }
        public string CMND { get; set; }
        public DateTime? NgayCap { get; set; }
        public string  NoiCap { get; set; }
        public string DuDinhDatTenCon { get; set; }
        public long DuDinhDatTenConId { get; set; }
        public string GioiTinh { get; set; }
        public double? CanNang { get; set; }
        public string GhiChu { get; set; }
        public DateTime? NgayCapGiayChungSinh { get; set; } // ngày thực hiện
        public long? NhanVienDoDeId { get; set; }
        public long? NhanVienGhiPhieuId { get; set; }
        public long? GiamDocChuyenMonId { get; set; }
        public List<FileDinhKemViewModel> FileDinhKems { get; set; }
        public int TrangThaiLuu { get; set; }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiGianDe { get; set; }
        public long? NoiTruHoSoKhacGiayChungSinhId { get; set; }
        public string TenNhanVienDoDe { get; set; }
        public string TenNhanVienGhiPhieu { get; set; }
        public string TenGiamDocChuyenMon { get; set; }
    }
    public class FileDinhKemViewModel : BaseViewModel
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Ma { get; set; }
        public string Ten { get; set; }
        public string TenGuid { get; set; }
        public long KichThuoc { get; set; }
        public string DuongDan { get; set; }
        public Enums.LoaiTapTin LoaiTapTin { get; set; }
        public string MoTa { get; set; }
        public string Uid { get; set; }
    }
    public class GiayChungSinhNewViewModel : BaseViewModel
    {
        public GiayChungSinhNewViewModel()
        {
            FileDinhKems = new List<FileDinhKemViewModel>();
            NoiTruHoSoKhacFileDinhKems = new List<NoiTruHoSoKhacFileDinhKemViewModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public DateTime? ThoiDiemThucHien { get; set; }
        public Enums.LoaiHoSoDieuTriNoiTru LoaiHoSoDieuTriNoiTru { get; set; }
        public string ThongTinHoSo { get; set; }
        public long? NhanVienThucHienId { get; set; }
        public long? NoiThucHienId { get; set; }
        public List<FileDinhKemViewModel> FileDinhKems { get; set; }
        public List<NoiTruHoSoKhacFileDinhKemViewModel> NoiTruHoSoKhacFileDinhKems { get; set; }
    }
    public class DSGiayChungSinhNewConVo
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string DuDinhDatTenCon { get; set; }
        public string GioiTinh { get; set; }
        public double? CanNang { get; set; }
        public int TrangThaiLuu { get; set; }
        public string So { get; set; }
        public string QuyenSo { get; set; }
    }
    public class InGiayChungSinhQueryInfo
    {
        public long NoiTruHoSoKhacId { get; set; }
        public string Hosting { get; set; }
    }
    #endregion
    }
