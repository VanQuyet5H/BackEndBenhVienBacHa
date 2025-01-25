using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NgheNghiep;
using FluentValidation;

namespace Camino.Api.Models.NgheNghiep.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NgheNghiepViewModel>))]
    public class NgheNghiepViewModelValidator : AbstractValidator<NgheNghiepViewModel>
    {
        public NgheNghiepViewModelValidator(ILocalizationService localizationService, INgheNghiepService ngheNghiepService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await ngheNghiepService.IsTenNgheNghiepExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.TenVietTat)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.TenVietTat.Required"))
                .Length(0, 50).WithMessage(localizationService.GetResource("Common.TenVietTat.Range"))
                .MustAsync(async (model, input, s) => !await ngheNghiepService.IsTenVietTatExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.TenVietTat.Exists"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService.GetResource("Common.MoTa.Range"));
        }
    }
}
