using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;

namespace Camino.Api.Models.YeuCauTraVatTuTuBenhNhan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauTraVatTuTuBenhNhanViewModel>))]

    public class YeuCauTraVatTuTuBenhNhanViewModelValidator : AbstractValidator<YeuCauTraVatTuTuBenhNhanViewModel>
    {
        public YeuCauTraVatTuTuBenhNhanViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.KhoTraId)
                   .NotEmpty().WithMessage(localizationService.GetResource("YeuCauTraDuocPhamTuBenhNhan.KhoTraId.Required"));
        }
    }
}
