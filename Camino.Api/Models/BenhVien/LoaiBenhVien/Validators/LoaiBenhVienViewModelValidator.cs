using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BenhVien.LoaiBenhVien;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.BenhVien.LoaiBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LoaiBenhVienViewModel>))]
    public class LoaiBenhVienViewModelValidator
        : AbstractValidator<LoaiBenhVienViewModel>
    {
        public LoaiBenhVienViewModelValidator(
            ILocalizationService localizationService,
            ILoaiBenhVienService loaiBenhVienService
        )
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService
                .GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService
                .GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await loaiBenhVienService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService
                .GetResource("Common.MoTa.Range"));
        }
    }
}
