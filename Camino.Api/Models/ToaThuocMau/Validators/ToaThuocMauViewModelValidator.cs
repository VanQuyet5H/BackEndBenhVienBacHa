using Camino.Core.DependencyInjection.Attributes;
using FluentValidation;
using Camino.Services.Localization;

namespace Camino.Api.Models.ToaThuocMau.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ToaThuocMauViewModel>))]
    public class ToaThuocMauViewModelValidator : AbstractValidator<ToaThuocMauViewModel>
    {
        public ToaThuocMauViewModelValidator(ILocalizationService localizationService, IValidator<ToaThuocMauChiTietViewModel> toaThuocChiTietValidator)
        {
            RuleFor(x => x.Ten)
               .NotEmpty().WithMessage(localizationService.GetResource("ToaThuocMau.Ten.Required"));

            RuleFor(x => x.BacSiKeToaId)
               .NotEmpty().WithMessage(localizationService.GetResource("ToaThuocMau.BacSiKeToa.Required"));

            RuleForEach(x => x.ToaThuocMauChiTiets).SetValidator(toaThuocChiTietValidator);

        }
    }
}
