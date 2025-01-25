using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using Camino.Services.NgayLeTet;
using FluentValidation;

namespace Camino.Api.Models.NgayLeTet.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NgayLeTetViewModel>))]
    public class NgayLeTetViewModelValidator : AbstractValidator<NgayLeTetViewModel>
    {
        public NgayLeTetViewModelValidator(ILocalizationService localizationService, INgayLeTetService quocGiaService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(localizationService.GetResource("Common.Ten.Required"))
                .Length(0, 250).WithMessage(localizationService.GetResource("Common.Ten.Range"));

            RuleFor(x => x.Ngay)
                .NotEmpty().WithMessage(localizationService.GetResource("NgayLeTet.NgayLeTet.Ngay.Required"));

            RuleFor(x => x.Thang)
              .NotEmpty().WithMessage(localizationService.GetResource("NgayLeTet.NgayLeTet.Thang.Required"));
        }
    }
}