using Camino.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.Domain.ValueObject.KhamBenhs;
using Camino.Core.Helpers;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru
{
    public class DieuTriNoiTruPhieuDieuTriViewModel : BaseViewModel
    {
        public long NoiTruBenhAnId { get; set; }
        public long NhanVienLapId { get; set; }
        public long KhoaPhongDieuTriId { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public DateTime? ThoiDiemThamKham { get; set; }
        public long? ChanDoanChinhICDId { get; set; }
        public string ChanDoanChinhGhiChu { get; set; }
        public string DienBien { get; set; }
        public Enums.CheDoChamSoc? CheDoChamSoc { get; set; }
        public string GhiChuChamSoc { get; set; }
        public long? CheDoAn { get; set; }
    }

    public class ChiTietDieuTriNoiTruViewModel : BaseViewModel
    {
        public ChiTietDieuTriNoiTruViewModel()
        {
            GoiDichVus = new List<GoiDichVuTheoBenhNhanGridVo>();
        }
        public string MaYeuCauTiepNhan { get; set; }
        public long? BenhNhanId { get; set; }
        public string MaBenhNhan { get; set; }
        public string HoTen { get; set; }
        public string Tuoi { get; set; }
        public string GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string NgheNghiep { get; set; }
        public string SoBenhAn { get; set; }
        public string LoaiBenhAn { get; set; }
        public Enums.LoaiBenhAn LoaiBenhAnEnum { get; set; }
        public string Khoa { get; set; }
        public long? KhoaId { get; set; }
        public string BacSiDieuTri { get; set; }
        public string Phong { get; set; }
        public string Giuong { get; set; }


        public Enums.EnumTrangThaiDieuTriNoiTru TrangThaiId { get; set; }
        public string TrangThai => TrangThaiId.GetDescription();
        public string SoTaiKhoan { get; set; }
        public int? MucHuong { get; set; }

        public decimal? SoDuTaiKhoan { get; set; }
        public decimal? SoTienConLai { get; set; }
        public bool? CoBHYT { get; set; }
        public string DoiTuong => CoBHYT == true ? "BHYT(" + MucHuong + "%)" : "Viện phí";

        public bool? KetThucBenhAn { get; set; }
        public bool? DaChiDinhGiuongVaBacSi => !string.IsNullOrEmpty(BacSiDieuTri) && !string.IsNullOrEmpty(Giuong);
        public bool? DaQuyetToan { get; set; }
        public bool? CoDichVuKhuyenMai { get; set; }

        public List<ThongTinBALink> BenhAnCons { get; set; }
        public ThongTinBALink BenhAnMe { get; set; }
        public bool? CungKhoaDangNhap { get; set; }

        public bool IsDaChiDinhBacSi { get; set; }
        public bool IsDaChiDinhGiuong { get; set; }
        public long KhoaPhuSanId { get; set; }

        public long? YeuCauTiepNhanNgoaiTruId { get; set; }

        public List<GoiDichVuTheoBenhNhanGridVo> GoiDichVus { get; set; }

        //BVHD-3800
        public bool? LaCapCuu { get; set; }

        //BVHD-3941
        public string TenCongTyBaoHiemTuNhan { get; set; }

        //BVHD-3960
        public string TenHinhThucDen { get; set; }
        public string TenNoiGioiThieu { get; set; }
        public bool? LaHinhThucDenGioiThieu { get; set; }
        public string HinhThucDenDisplay => LaHinhThucDenGioiThieu.GetValueOrDefault() ? $"{TenHinhThucDen} / {TenNoiGioiThieu}" : TenHinhThucDen;
    }

    public class ThongTinBALink
    {
        public long? BenhAnId { get; set; }
        public string SoBenhAn { get; set; }

    }

    public class PhieuKhamThamKhamViewModel : BaseViewModel
    {
        public PhieuKhamThamKhamViewModel()
        {
            NoiTruThamKhamChanDoanKemTheos = new List<PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModel>();
            KetQuaSinhHieus = new List<PhieuThamKhamKetQuaSinhHieuViewModel>();
            DienBiens = new List<PhieuThamKhamDienBienViewModel>();
            ThoiGianDieuTriSoSinhViewModels = new List<ThoiGianDieuTriSoSinhViewModel>();
        }
        //public long YeuCauTiepNhanId { get; set; }
        public DateTime NgayDieuTri { get; set; }
        public long PhieuDieuTriId { get; set; }
        #region thong tin
        public bool LaCapCuu { get; set; }
        public string KhoaChiDinh { get; set; }
        public long? KhoaChiDinhId { get; set; }

        public string Phong { get; set; }
        public string Giuong { get; set; }
        public string BSDieuTri { get; set; }
        public string DieuDuong { get; set; }
        public string NgayYLenh { get; set; }
        public string ChuanDoanNhapVien { get; set; }

        public bool? BenhNhanCapCuu { get; set; }


        public DateTime? ThoiDiemThamKham { get; set; }
        #endregion thong tin
        public DateTime? ThoiDiemNhapVien { get; set; }

        public string DienBien { get; set; }
        public Enums.CheDoChamSoc? CheDoChamSoc { get; set; }
        public string GhiChuChamSoc { get; set; }
        public long? CheDoAnId { get; set; }
        public string TenCheDoAn { get; set; }

        public long? ChanDoanChinhICDId { get; set; }
        public string ChanDoanChinhICDModelText { get; set; }
        public string ChanDoanChinhGhiChu { get; set; }
        public decimal? SoNgayDieuTriBenhAnSoSinh { get; set; }

        public List<PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModel> NoiTruThamKhamChanDoanKemTheos { get; set; }
        public List<PhieuThamKhamKetQuaSinhHieuViewModel> KetQuaSinhHieus { get; set; }
        public List<PhieuThamKhamDienBienViewModel> DienBiens { get; set; }
        public List<ThoiGianDieuTriSoSinhViewModel> ThoiGianDieuTriSoSinhViewModels { get; set; }
    }

    public class PhieuThamKhamDienBienViewModel
    {
        public long IdView { get; set; }
        public string DienBien { get; set; }
        public string YLenh { get; set; }
        public long? CheDoChamSocId { get; set; }
        public long? CheDoAnId { get; set; }
        public string CheDoChamSoc { get; set; }
        public string CheDoAn { get; set; }
        public DateTime? ThoiGian { get; set; }
        public string ThoiGianDisplay => ThoiGian?.ApplyFormatDateTime();

        public bool? IsUpdate { get; set; }
        public long? DienBienLastUserId { get; set; }
        public long? YLenhLastUserId { get; set; }

    }

    public class UpdateDieuTriSoSinhRaVienViewModel 
    {
        public UpdateDieuTriSoSinhRaVienViewModel()
        { ThoiGianDieuTriSoSinhRaVienViewModel = new List<ThoiGianDieuTriSoSinhRaVienViewModel>(); }
        public List<ThoiGianDieuTriSoSinhRaVienViewModel> ThoiGianDieuTriSoSinhRaVienViewModel { get; set; }
    }
    public class ThoiGianDieuTriSoSinhRaVienViewModel : BaseViewModel
    {
        public ThoiGianDieuTriSoSinhRaVienViewModel()
        { ThoiGianDieuTriSoSinhViewModels = new List<ThoiGianDieuTriSoSinhViewModel>(); }
        public long? NoiTruBenhAnId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
        public List<ThoiGianDieuTriSoSinhViewModel> ThoiGianDieuTriSoSinhViewModels { get; set; }
    }

    public class ThoiGianDieuTriSoSinhViewModel : BaseViewModel
    {
        public int? GioBatDau { get; set; }
        public int? GioKetThuc { get; set; }
        public string GhiChuDieuTri { get; set; }
        public string NgayDieuTriString => NgayDieuTri.ToString("dd/MM/yyyy");
        public DateTime NgayDieuTri { get; set; }
        public long? NoiTruBenhAnId { get; set; }
        public long? NoiTruPhieuDieuTriId { get; set; }
    }
    public class PhieuThamKhamNoiTruThamKhamChanDoanKemTheoViewModel : BaseViewModel
    {
        //public long? ICDId { get; set; }
        //public string ICDModelText { get; set; }
        //public string GhiChu { get; set; }
        public long NoiTruPhieuDieuId { get; set; }
        public long? ICDId { get; set; }
        public string GhiChu { get; set; }
        public string TenICD { get; set; }

    }

    public class ParamsThongTinPhieuKhamModel
    {
        public long yeuCauTiepNhanId { get; set; }
        public long phieuDieuTriId { get; set; }
    }

    public class PhieuThamKhamKetQuaSinhHieuViewModel : BaseViewModel
    {
        public int? NhipTim { get; set; }

        public int? NhipTho { get; set; }

        public double? ThanNhiet { get; set; }

        public int? HuyetApTamThu { get; set; }

        public int? HuyetApTamTruong { get; set; }

        public double? ChieuCao { get; set; }

        public double? CanNang { get; set; }

        public double? Bmi { get; set; }
        public double? Glassgow { get; set; }
        public double? SpO2 { get; set; }

        public DateTime? ThoiDiemThucHien { get; set; }
        public DateTime? ThoiDiemNhapVien { get; set; }

        public string ThoiDiemThucHienDisplay => ThoiDiemThucHien?.ApplyFormatDateTime();
    }
    //public class NgayDieuTriModel
    //{
    //    public long KhoaId { get; set; }
    //    public long Id { get; set; }
    //    public DateTime Date { get; set; }
    //}
    public class ThemSuatAnViewModel : BaseViewModel
    {
        public long YeuCauTiepNhanId { get; set; }
        public long NoiTruPhieuDieuTriId { get; set; }
        public Enums.DoiTuongSuDung? DoiTuongSuDung { get; set; }
        public BuaAn? BuaAn { get; set; }
        public int? SoLan { get; set; }
        public long? DichVuKyThuatBenhVienId { get; set; }

    }

    public class InPhieuThamKhamTheoNgayModel 
    {
        public InPhieuThamKhamTheoNgayModel()
        {
            DienBienModels = new List<DienBienModel>();
        }
        public long YeuCauTiepNhanId { get; set; }
        public long PhieuDieuTriId { get; set; }
      
        public List<DienBienModel> DienBienModels { get; set; }

    }
    public class DienBienModel
    { 
        public long Id { get; set; }
        public DateTime ThoiGian { get; set; }
    }
}
