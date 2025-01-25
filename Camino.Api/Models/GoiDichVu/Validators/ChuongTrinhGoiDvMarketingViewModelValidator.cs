using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.GoiDichVuMarketings;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.GoiDichVu.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ChuongTrinhGoiDvMarketingViewModel>))]
    public class ChuongTrinhGoiDvMarketingViewModelValidator : AbstractValidator<ChuongTrinhGoiDvMarketingViewModel>
    {
        public ChuongTrinhGoiDvMarketingViewModelValidator(ILocalizationService localizationService, IChuongTrinhMarketingGoiDvService chuongTrinhMarketingGoiDvService)
        {
            RuleFor(x => x.Ten)
                .MustAsync(async (model, input, s) => !await chuongTrinhMarketingGoiDvService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.Ma)
                .MustAsync(async (model, input, s) => !await chuongTrinhMarketingGoiDvService.IsMaExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));
        }
    }
}
