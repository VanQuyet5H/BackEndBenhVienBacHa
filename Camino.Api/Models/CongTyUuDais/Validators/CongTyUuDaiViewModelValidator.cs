using Camino.Api.Models.YeuCauKhamBenh;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.CongTyUuDais;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.CongTyUuDais.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<CongTyUuDaiViewModel>))]
    public class CongTyUuDaiViewModelValidator : AbstractValidator<CongTyUuDaiViewModel>
    {
        public CongTyUuDaiViewModelValidator(ILocalizationService localizationService, ICongTyUuDaiService congTyUuDaiService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 500).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await congTyUuDaiService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));
        }
    }
}
