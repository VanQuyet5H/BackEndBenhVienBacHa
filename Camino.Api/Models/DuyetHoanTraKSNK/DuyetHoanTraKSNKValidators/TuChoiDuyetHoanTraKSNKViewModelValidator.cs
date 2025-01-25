using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DuyetHoanTraKSNK.DuyetHoanTraKSNKValidators
{
    [TransientDependency(ServiceType = typeof(IValidator<TuChoiDuyetHoanTraKSNKViewModel>))]
    public class TuChoiDuyetHoanTraKSNKViewModelValidator : AbstractValidator<TuChoiDuyetHoanTraKSNKViewModel>
    {
        public TuChoiDuyetHoanTraKSNKViewModelValidator(ILocalizationService localizationService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.LyDoHuy)
                .NotNull().WithMessage(localizationService.GetResource("DuyetHoanTra.LyDoHuy.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("DuyetHoanTra.LyDoHuy.Required"));
        }
    }
}
