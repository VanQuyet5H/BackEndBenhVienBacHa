using System;
using System.Collections.Generic;
using System.ComponentModel;
using Camino.Core.Domain;
using Camino.Core.Domain.ValueObject.KhamDoan;
using Camino.Core.Helpers;

namespace Camino.Api.Models.KhamDoan
{
    public class YeuCauNhanSuKhamSucKhoeViewModel : BaseViewModel
    {
        public YeuCauNhanSuKhamSucKhoeViewModel()
        {
            HopDongKhamSucKhoeDiaDiems = new List<HopDongKhamSucKhoeDiaDiemViewModel>();
            YeuCauNhanSuKhamSucKhoeChiTiets = new List<YeuCauNhanSuKhamSucKhoeChiTietViewModel>();
            NhanSuBiLoaiBo = new List<long>();
        }

        public long HopDongKhamSucKhoeId { get; set; }

        public long CongTyId { get; set; }

        public string CongTy { get; set; }

        public int SoNguoiKham { get; set; }

        public DateTime NgayHieuLuc { get; set; }

        public string NgayHieuLucDisplay => NgayHieuLuc.ApplyFormatDate();

        public DateTime? NgayKetThuc { get; set; }

        public string NgayKetThucDisplay => NgayKetThuc != null ? NgayKetThuc.GetValueOrDefault().ApplyFormatDate():"";

        public int TongSoBs { get; set; }

        public int TongSoDd { get; set; }

        public int TongNvKhac { get; set; }

        public long? NhanVienGuiYeuCauId { get; set; }

        public string NhanVienGui { get; set; }

        public DateTime? NgayGuiYeuCau { get; set; }

        public bool? DuocKHTHDuyet { get; set; }

        public DateTime? NgayKHTHDuyet { get; set; }

        public string NgayKhthDuyetDisplay => NgayKHTHDuyet != null
            ? NgayKHTHDuyet.GetValueOrDefault().ApplyFormatDateTime()
            : string.Empty;

        public long? NhanVienKHTHDuyetId { get; set; }

        public string NhanVienKhthDuyet { get; set; }

        public string LyDoKHTHKhongDuyet { get; set; }

        public bool? DuocNhanSuDuyet { get; set; }

        public DateTime? NgayNhanSuDuyet { get; set; }

        public string NgayNhanSuDuyetDisplay => NgayNhanSuDuyet != null
            ? NgayNhanSuDuyet.GetValueOrDefault().ApplyFormatDateTime()
            : string.Empty;

        public long? NhanVienNhanSuDuyetId { get; set; }

        public string NhanVienNhanSuDuyet { get; set; }

        public string LyDoNhanSuKhongDuyet { get; set; }

        public bool? DuocGiamDocDuyet { get; set; }

        public DateTime? NgayGiamDocDuyet { get; set; }

        public string NgayGiamDocDuyetDisplay => NgayGiamDocDuyet != null
            ? NgayGiamDocDuyet.GetValueOrDefault().ApplyFormatDateTime()
            : string.Empty;

        public long? GiamDocId { get; set; }

        public string GiamDoc { get; set; }

        public string LyDoGiamDocKhongDuyet { get; set; }

        public EnumTrangThaiKhamDoan? TrangThai { get; set; }

        public string TenTinhTrang => TrangThai != null ? TrangThai.GetDescription() : string.Empty;

        public List<HopDongKhamSucKhoeDiaDiemViewModel> HopDongKhamSucKhoeDiaDiems { get; set; }

        public List<YeuCauNhanSuKhamSucKhoeChiTietViewModel> YeuCauNhanSuKhamSucKhoeChiTiets { get; set; }

        public List<long> NhanSuBiLoaiBo { get; set; }

        public bool? IsDuyet { get; set; }
    }

    public class HopDongKhamSucKhoeDiaDiemViewModel : BaseViewModel
    {
        public string DiaDiem { get; set; }

        public string CongViec { get; set; }

        public DateTime? Ngay { get; set; }

        public string NgayDisplay => Ngay != null ? Ngay.GetValueOrDefault().ApplyFormatDate() : string.Empty;

        public string GhiChu { get; set; }
    }

    public class YeuCauNhanSuKhamSucKhoeChiTietViewModel : BaseViewModel
    {
        public YeuCauNhanSuKhamSucKhoeChiTietViewModel()
        {
            NhanSuKhamSucKhoeTaiLieuDinhKem = new List<NhanSuKhamSucKhoeTaiLieuDinhKemViewModel>();
        }

        public Enums.NguonNhanSu NguonNhanSu { get; set; }

        public string HoTen { get; set; }

        public long? NhanVienId { get; set; }

        public LoaiNhanVien? LoaiNhanVien { get; set; }

        public string DonVi { get; set; }

        public string ViTriLamViec { get; set; }

        public string SoDienThoai { get; set; }

        public Enums.DoiTuongNhanSu DoiTuongLamViec { get; set; }

        public string DoiTuongLamViecDisplay => DoiTuongLamViec.GetDescription();

        public long? NguoiGioiThieuId { get; set; }

        public string NguoiGioiThieu { get; set; }

        public List<NhanSuKhamSucKhoeTaiLieuDinhKemViewModel> NhanSuKhamSucKhoeTaiLieuDinhKem { get; set; }

        public long? NhanSuKhamSucKhoeTaiLieuDinhKemId { get; set; }

        public string GhiChu { get; set; }

        public bool? IsCreate { get; set; }

        public bool? IsUpdate { get; set; }       
    }

    public class NhanSuKhamSucKhoeTaiLieuDinhKemViewModel : BaseViewModel
    {
        public string Ma { get; set; }
        
        public string Ten { get; set; }
        
        public string TenGuid { get; set; }
        
        public long KichThuoc { get; set; }
        
        public string DuongDan { get; set; }
        
        public int LoaiTapTin { get; set; }
        
        public string MoTa { get; set; }
        public bool IsExistingFile =>true;
    }

    public enum LoaiNhanVien
    {
        [Description("Bác sĩ")]
        BacSi = 1,
        [Description("Điều dưỡng")]
        DieuDuong = 2,
        [Description("Nhân viên khác")]
        NhanVienKhac = 3
    }
}
