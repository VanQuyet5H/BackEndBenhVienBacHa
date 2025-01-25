using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.Voucher;
using FluentValidation;

namespace Camino.Api.Models.Voucher.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<VoucherMarketingViewModel>))]
    public class VoucherMarketingViewModelValidator : AbstractValidator<VoucherMarketingViewModel>
    {
        public VoucherMarketingViewModelValidator(ILocalizationService localizationService, IVoucherService voucherService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.Ma)
                .NotNull().WithMessage(localizationService.GetResource("Marketing.Voucher.Ma.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("Marketing.Voucher.Ma.Required"))
                .Length(0, 20).WithMessage(localizationService.GetResource("Marketing.Voucher.Ma.Range.5"))
                .MustAsync(async (model, input, source) => !await voucherService.IsMaExists(model.Ma, model.Id)).WithMessage(localizationService.GetResource("Marketing.Voucher.Ma.Existed"))
                .When(p => p.IsThemHoacLuuDichVu);

            RuleFor(p => p.Ten)
                .NotNull().WithMessage(localizationService.GetResource("Marketing.Voucher.Ten.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("Marketing.Voucher.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Marketing.Voucher.Ten.Range.250"))
                .When(p => p.IsThemHoacLuuDichVu);

            RuleFor(p => p.SoLuongPhatHanh)
                .NotNull().WithMessage(localizationService.GetResource("Marketing.Voucher.SoLuong.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("Marketing.Voucher.SoLuong.Required"))
                .When(p => p.IsThemHoacLuuDichVu, ApplyConditionTo.AllValidators);

            RuleFor(p => p.TuNgay)
                .NotNull().WithMessage(localizationService.GetResource("Marketing.Voucher.TuNgay.Required"))
                .When(p => p.IsThemHoacLuuDichVu);

            RuleFor(p => p.DenNgay)
                .Must((model, s) => voucherService.CompareTuNgayDenNgay(model.TuNgay, model.DenNgay)).WithMessage(localizationService.GetResource("Marketing.Voucher.TuNgay.GreaterThan.DenNgay"))
                .When(p => p.IsThemHoacLuuDichVu && p.TuNgay != null);

            RuleFor(p => p.GhiChu)
                .Length(0, 4000).WithMessage(localizationService.GetResource("Marketing.Voucher.MoTa.Range.4000"))
                .When(p => p.IsThemHoacLuuDichVu);
        }
    }
}
