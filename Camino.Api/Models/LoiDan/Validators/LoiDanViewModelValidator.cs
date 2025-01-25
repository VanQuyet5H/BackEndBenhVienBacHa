using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.LoiDan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<LoiDanViewModel>))]
    public class LoiDanViewModelValidator : AbstractValidator<LoiDanViewModel>
    {
        public LoiDanViewModelValidator(ILocalizationService iLocalizationService)
        {
            RuleFor(x => x.IcdId)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("LoiDan.IcdId.Required"));

            RuleFor(x => x.LoiDanCuaBacSi)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("LoiDan.LoiDanCuaBacSi.Required"))
                .Length(0, 5000).WithMessage(iLocalizationService.GetResource("LoiDan.LoiDanCuaBacSi.Range"));
        }
    }
}
