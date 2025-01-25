using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.BenhVien.CapQuanLyBenhVien;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.BenhVien.CapQuanLyBenhVien.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<CapQuanLyBenhVienViewModel>))]
    public class CapQuanLyBenhVienViewModelValidator
        : AbstractValidator<CapQuanLyBenhVienViewModel>
    {
        public CapQuanLyBenhVienViewModelValidator(
                   ILocalizationService localizationService,
                   ICapQuanLyBenhVienService capQuanLyBenhVienService
               )
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService
                .GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService
                .GetResource("Common.Ten.Range"))
                .MustAsync(async (model, input, s) => !await capQuanLyBenhVienService.IsTenExists(
                    !string.IsNullOrEmpty(input) ? input.TrimEnd().TrimStart() : input,
                    model.Id).ConfigureAwait(false))
                .WithMessage(localizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.MoTa)
                .Length(0, 2000).WithMessage(localizationService
                .GetResource("Common.MoTa.Range"));
        }
    }
}
