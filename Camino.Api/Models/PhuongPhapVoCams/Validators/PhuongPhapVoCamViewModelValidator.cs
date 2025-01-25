using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.PhuongPhapVoCam;
using FluentValidation;

namespace Camino.Api.Models.PhuongPhapVoCams.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhuongPhapVoCamViewModel>))]
    public class PhuongPhapVoCamViewModelValidator : AbstractValidator<PhuongPhapVoCamViewModel>
    {
        public PhuongPhapVoCamViewModelValidator(ILocalizationService localizationService, IPhuongPhapVoCamService phuongPhapVoCamService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"));

            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ma.Required"))
                .MustAsync(async (model, input, s) => !await phuongPhapVoCamService.IsMaExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));
        }
    }
}
