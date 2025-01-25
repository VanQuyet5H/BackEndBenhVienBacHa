using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;
using static Camino.Core.Domain.Enums;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<EkipDieuTriViewModel>))]
    public class EkipDieuTriViewModelValidator : AbstractValidator<EkipDieuTriViewModel>
    {
        public EkipDieuTriViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.NoiTruBenhAnId)
                .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.NoiTruBenhAnId.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.NoiTruBenhAnId.Required"));

            RuleFor(p => p.BacSiId)
                .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.BacSiId.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.BacSiId.Required"));

            RuleFor(p => p.DieuDuongId)
                .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.DieuDuongId.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.DieuDuongId.Required"));

            RuleFor(p => p.KhoaPhongDieuTriId)
                .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.KhoaPhongDieuTriId.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.KhoaPhongDieuTriId.Required"));

            RuleFor(p => p.TuNgay)
                .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.TuNgay.Required"))
                .Must((model, s) => dieuTriNoiTruService.KiemTraTuNgayVoiNgayNhapVien(model.TuNgay, model.NoiTruBenhAnId.GetValueOrDefault())).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.TuNgay.GreaterOrEqualThan.NgayNhapVien"))
                .Must((model, s) => dieuTriNoiTruService.KiemTraTonTaiLichBacSiDieuTriTuNgay(model.TuNgay, model.NoiTruBenhAnId.GetValueOrDefault(), model.Id)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.TuNgay.Existed"));

            //RuleFor(p => p.DenNgay)
            //    .Must((model, s) => dieuTriNoiTruService.CompareTuNgayDenNgay(model.TuNgay, model.DenNgay)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.DenNgay.GreaterThan.TuNgay")).When(p => p.TuNgay != null)
            //    .Must((model, s) => dieuTriNoiTruService.KiemTraTonTaiLichBacSiDieuTriDenNgay(model.DenNgay, model.NoiTruBenhAnId.GetValueOrDefault(), model.Id)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.BsDieuTri.DenNgay.Existed"));
        }
    }

    [TransientDependency(ServiceType = typeof(IValidator<CapGiuongViewModel>))]
    public class CapGiuongViewModelValidator : AbstractValidator<CapGiuongViewModel>
    {
        public CapGiuongViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.GiuongBenhId)
                .NotNull().WithMessage(localizationService.GetResource("NoiTruBenhAn.GiuongId.Required")).When(p => p.DoiTuongSuDung != DoiTuongSuDung.NguoiNha, ApplyConditionTo.CurrentValidator);

            RuleFor(p => p.LoaiGiuong)
                .NotNull().WithMessage(localizationService.GetResource("NoiTruBenhAn.LoaiGiuong.Required")).When(p => p.DoiTuongSuDung != DoiTuongSuDung.NguoiNha, ApplyConditionTo.CurrentValidator);

            RuleFor(p => p.ThoiDiemBatDauSuDung)
                .NotNull().WithMessage(localizationService.GetResource("NoiTruBenhAn.ThoiGianNhan.Required"))
                //.Must((model, s) => dieuTriNoiTruService.KiemTraTonTaiLichCapGiuongThoiGianNhan(model.ThoiDiemKetThucSuDung, model.YeuCauTiepNhanId, model.Id)).WithMessage(localizationService.GetResource("existed"))
                .Must((model, s) => dieuTriNoiTruService.KiemTraThoiGianNhanGiuongVoiNgayNhapVien(model.ThoiDiemBatDauSuDung, model.YeuCauTiepNhanId)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.CapGiuong.ThoiGianNhan.GreaterOrEqualThan.NgayNhapVien"))
                //.Must((model, s) => dieuTriNoiTruService.KiemTraThoiGianNhanGiuongVoiThoiDiemChiDinhGiuong(model.ThoiDiemBatDauSuDung, model.YeuCauTiepNhanId)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.CapGiuong.ThoiGianNhan.GreaterOrEqualThan.ThoiDiemVaoKhoa")).When(p => p.Id == 0, ApplyConditionTo.CurrentValidator)
                .Must((model, s) => dieuTriNoiTruService.KiemTraThoiGianNhanTonTaiBenhNhanTrongLichChuyenPhong(model.ThoiDiemBatDauSuDung, model.DoiTuongSuDung, model.YeuCauTiepNhanId, model.Id)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.DoiTuongBenhNhan.Existed"));

            RuleFor(p => p.ThoiDiemKetThucSuDung)
                .Must((model, s) => dieuTriNoiTruService.CompareTuNgayDenNgay(model.ThoiDiemBatDauSuDung, model.ThoiDiemKetThucSuDung)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ThoiGianTra.GreaterThan.ThoiGianNhan")).When(p => p.ThoiDiemBatDauSuDung != null);
                //.Must((model, s) => dieuTriNoiTruService.KiemTraTonTaiLichCapGiuongThoiGianTra(model.ThoiDiemKetThucSuDung, model.YeuCauTiepNhanId, model.Id)).WithMessage(localizationService.GetResource("existed"));
                //.Must((model, s) => dieuTriNoiTruService.KiemTraThoiGianTraTonTaiBenhNhanTrongLichChuyenPhong(model.ThoiDiemKetThucSuDung, model.DoiTuongSuDung, model.YeuCauTiepNhanId, model.Id)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.DoiTuongBenhNhan.Existed"));

            RuleFor(p => p.DichVuGiuongBenhVienId)
                .NotEmpty().WithMessage(localizationService.GetResource("NoiTruBenhAn.DichVuGiuongId.Required"));

            RuleFor(p => p.DoiTuongSuDung)
                .NotNull().WithMessage(localizationService.GetResource("NoiTruBenhAn.DoiTuongSuDung.Required"));

            RuleFor(p => p.GhiChu)
                .Length(0, 1000).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.LyDoChuyen.Range.1000"));
        }
    }

    #region Chi tiết sử dụng giường theo ngày
    //[TransientDependency(ServiceType = typeof(IValidator<ChiTietSuDungGiuongTheoNgayViewModel>))]
    //public class ChiTietSuDungGiuongTheoNgayViewModelValidator : AbstractValidator<ChiTietSuDungGiuongTheoNgayViewModel>
    //{
    //    public ChiTietSuDungGiuongTheoNgayViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService, IValidator<SuDungGiuongTheoNgayViewModel> suDungGiuongTheoNgayValidator)
    //    {
    //        this.CascadeMode = CascadeMode.StopOnFirstFailure;

    //        RuleForEach(p => p.SuDungGiuongTheoNgays).SetValidator(suDungGiuongTheoNgayValidator);
    //    }
    //}

    //[TransientDependency(ServiceType = typeof(IValidator<SuDungGiuongTheoNgayViewModel>))]
    //public class SuDungGiuongTheoNgayViewModelValidator : AbstractValidator<SuDungGiuongTheoNgayViewModel>
    //{
    //    public SuDungGiuongTheoNgayViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService, IValidator<ChiTietGiuongBenhVienChiPhiViewModel> chiTietGiuongBenhVienChiPhiValidator)
    //    {
    //        this.CascadeMode = CascadeMode.StopOnFirstFailure;

    //        RuleForEach(p => p.ChiTietGiuongBenhVienChiPhis).SetValidator(chiTietGiuongBenhVienChiPhiValidator);
    //    }
    //}

    //[TransientDependency(ServiceType = typeof(IValidator<ChiTietGiuongBenhVienChiPhiViewModel>))]
    //public class ChiTietGiuongBenhVienChiPhiViewModelValidator : AbstractValidator<ChiTietGiuongBenhVienChiPhiViewModel>
    //{
    //    public ChiTietGiuongBenhVienChiPhiViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
    //    {
    //        this.CascadeMode = CascadeMode.StopOnFirstFailure;

    //        RuleFor(p => p.LoaiChiPhiGiuongBenh)
    //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.LoaiChiPhiGiuongBenh.Required")).When(p => p.isCreated != true);

    //        RuleFor(p => p.GiuongChuyenDenId)
    //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.GiuongChuyenDen.Required")).When(p => p.DoiTuong == DoiTuongSuDung.BenhNhan);

    //        RuleFor(p => p.DichVuGiuongId)
    //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.DichVuGiuong.Required"));

    //        RuleFor(p => p.SoLuong)
    //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.SoLuong.Required"))
    //            .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.SoLuong.Min"));

    //        RuleFor(p => p.SoLuongGhep)
    //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.SoLuongGhep.Required"));

    //        RuleFor(p => p.DoiTuong)
    //            .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChiTietSuDung.DoiTuong.Required"));

    //        //RuleFor(p => p.DonGia)
    //        //    .NotNull().WithMessage(localizationService.GetResource("LoaiChiPhiGiuongBenh null"));

    //        //RuleFor(p => p.ThanhTien)
    //        //    .NotNull().WithMessage(localizationService.GetResource("LoaiChiPhiGiuongBenh null"));
    //    }
    //}
    #endregion

    [TransientDependency(ServiceType = typeof(IValidator<KhoaPhongDieuTriViewModel>))]
    public class KhoaPhongDieuTriViewModelValidator : AbstractValidator<KhoaPhongDieuTriViewModel>
    {
        public KhoaPhongDieuTriViewModelValidator(ILocalizationService localizationService, IDieuTriNoiTruService dieuTriNoiTruService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.NoiTruBenhAnId)
                .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.NoiTruBenhAnId.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.NoiTruBenhAnId.Required"));

            RuleFor(p => p.KhoaPhongChuyenDenId)
                .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.KhoaPhongChuyenDenId.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.KhoaPhongChuyenDenId.Required"))
                .Must((model, s) => dieuTriNoiTruService.KiemTraKhoaChuyenDenKhacKhoaChuyenDi(model.ThoiDiemVaoKhoa, model.NoiTruBenhAnId.GetValueOrDefault(), model.KhoaPhongChuyenDenId, model.Id)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.KhoaPhongChuyenDenId.Duplicated"));

            RuleFor(p => p.ThoiDiemVaoKhoa)
                .NotNull().WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.ThoiDiemVaoKhoa.Required"))
                .Must((model, s) => dieuTriNoiTruService.KiemTraThoiDiemVaoKhoaVoiNgayNhapVien(model.ThoiDiemVaoKhoa, model.NoiTruBenhAnId.GetValueOrDefault())).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.ThoiDiemVaoKhoa.GreaterOrEqualThan.NgayNhapVien"))
                .Must((model, s) => dieuTriNoiTruService.KiemTraTonTaiLichChuyenPhong(model.ThoiDiemVaoKhoa, model.NoiTruBenhAnId.GetValueOrDefault(), model.Id)).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.ThoiDiemVaoKhoa.Existed"));

            RuleFor(p => p.ChanDoanVaoKhoaGhiChu)
                .Length(0, 4000).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.ChanDoanVaoKhoaGhiChu.Length.4000"));

            RuleFor(p => p.LyDoChuyenKhoa)
                .Length(0, 1000).WithMessage(localizationService.GetResource("DieuTriNoiTru.PhongGiuongBsDieuTri.ChuyenKhoa.LyDoChuyenKhoa.Length.1000"));
        }
    }
}
