using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.PhamViHanhNghe;
using FluentValidation;

namespace Camino.Api.Models.PhamViHanhNghe.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhamViHanhNgheViewModel>))]
    public class PhamViHanhNgheViewModelValidator : AbstractValidator<PhamViHanhNgheViewModel>
    {
        public PhamViHanhNgheViewModelValidator(ILocalizationService localizationService, IPhamViHanhNgheService phamViHanhNgheService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await phamViHanhNgheService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("Common.Ma.Range"))
                .MustAsync(async (model, input, s) => !await phamViHanhNgheService.IsMaExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));
        }
    }
}
