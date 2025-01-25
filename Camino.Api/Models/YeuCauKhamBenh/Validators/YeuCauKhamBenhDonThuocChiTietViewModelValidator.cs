using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using Camino.Services.YeuCauKhamBenh;
using FluentValidation;

namespace Camino.Api.Models.YeuCauKhamBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauKhamBenhDonThuocChiTietViewModel>))]
    public class YeuCauKhamBenhDonThuocChiTietViewModelValidator : AbstractValidator<YeuCauKhamBenhDonThuocChiTietViewModel>
    {
        public YeuCauKhamBenhDonThuocChiTietViewModelValidator(ILocalizationService localizationService, IYeuCauKhamBenhService yeuCauKhamBenhService)
        {
            RuleFor(x => x.SangDisplay)
                .Must((model, input, s) => string.IsNullOrEmpty(model.SangDisplay) || model.SangDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.TruaDisplay)
               .Must((model, input, s) => string.IsNullOrEmpty(model.TruaDisplay) || model.TruaDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.ChieuDisplay)
               .Must((model, input, s) => string.IsNullOrEmpty(model.ChieuDisplay) || model.ChieuDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.ToiDisplay)
               .Must((model, input, s) => string.IsNullOrEmpty(model.ToiDisplay) || model.ToiDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.LieuDungTrenNgayDisplay)
                .Must((model, input, s) => string.IsNullOrEmpty(model.LieuDungTrenNgayDisplay) || model.LieuDungTrenNgayDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.SoLanTrenVienDisplay)
                .Must((model, input, s) => string.IsNullOrEmpty(model.SoLanTrenVienDisplay) || model.SoLanTrenVienDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.SoLuong)
                .NotEmpty().WithMessage(localizationService.GetResource("BHYT.SoLuong.Required"))
                .Must((request, soLuongNhap, id) => {
                    if (soLuongNhap <= 0)
                        return false;
                    return true;
                }).WithMessage(localizationService.GetResource("NhapKhoDuocPhamChiTiet.SoLuongNhap.LessThan0"));

            RuleFor(x => x.DuocPhamId)
                .NotEmpty().WithMessage(localizationService.GetResource("YeuCauKhamBenh.DuocPham.Required"));

            RuleFor(x => x.GhiChu)
               .Length(0, 1000).WithMessage(localizationService.GetResource("Common.GhiChu.Range.1000"));
        }
    }
}
