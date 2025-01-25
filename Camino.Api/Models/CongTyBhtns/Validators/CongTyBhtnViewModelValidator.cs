using Camino.Core.DependencyInjection.Attributes;
using Camino.Core.Helpers;
using Camino.Services.CongTyBhtns;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.CongTyBhtns.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<CongTyBhtnViewModel>))]
    public class CongTyBhtnViewModelValidator : AbstractValidator<CongTyBhtnViewModel>
    {
        public CongTyBhtnViewModelValidator(ILocalizationService localizationService, ICongTyBhtnService congTyBhtnService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await congTyBhtnService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("Common.Ma.Range"))
                .MustAsync(async (model, input, s) => !await congTyBhtnService.IsMaExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.Email)
                .Must((model, s) => CommonHelper.IsMailValid(model.Email)).WithMessage(localizationService.GetResource("Common.WrongEmail"));
        }
    }
}
