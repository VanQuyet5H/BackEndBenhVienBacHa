using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;

namespace Camino.Api.Models.YeuCauTraDuocPhamTuBenhNhan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauTraDuocPhamTuBenhNhanViewModel>))]

    public class YeuCauTraDuocPhamTuBenhNhanViewModelValidator : AbstractValidator<YeuCauTraDuocPhamTuBenhNhanViewModel>
    {
        public YeuCauTraDuocPhamTuBenhNhanViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoTraId)
                   .NotEmpty().WithMessage(localizationService.GetResource("YeuCauTraDuocPhamTuBenhNhan.KhoTraId.Required"));
        }
    }
}
