using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuThamKhamDienBienViewModel>))]

    public class PhieuThamKhamDienBienViewModelValidator : AbstractValidator<PhieuThamKhamDienBienViewModel>
    {
        public PhieuThamKhamDienBienViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DienBien)
              .NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.DienBien.Required"));

            RuleFor(x => x.ThoiGian)
             .NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.ThoiGian.Required"));
        }
    }
}
