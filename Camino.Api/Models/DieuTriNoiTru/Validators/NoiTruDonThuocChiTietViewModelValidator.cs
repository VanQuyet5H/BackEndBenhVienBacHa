using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NoiTruDonThuocChiTietViewModel>))]

    public class NoiTruDonThuocChiTietViewModelValidator : AbstractValidator<NoiTruDonThuocChiTietViewModel>
    {
        public NoiTruDonThuocChiTietViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.SangDisplay)
                .Must((model, input, s) => string.IsNullOrEmpty(model.SangDisplay) || model.SangDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.TruaDisplay)
               .Must((model, input, s) => string.IsNullOrEmpty(model.TruaDisplay) || model.TruaDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.ChieuDisplay)
               .Must((model, input, s) => string.IsNullOrEmpty(model.ChieuDisplay) || model.ChieuDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

            RuleFor(x => x.ToiDisplay)
               .Must((model, input, s) => string.IsNullOrEmpty(model.ToiDisplay) || model.ToiDisplay.IsNumberFraction()).WithMessage(localizationService.GetResource("YeuCauKhamBenh.CacBuoiTrongNgay.Format"));

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
