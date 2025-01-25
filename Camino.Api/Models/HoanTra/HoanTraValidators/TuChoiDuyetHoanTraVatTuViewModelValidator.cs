using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.HoanTra.HoanTraValidators
{
    [TransientDependency(ServiceType = typeof(IValidator<TuChoiDuyetHoanTraVatTuViewModel>))]
    public class TuChoiDuyetHoanTraVatTuViewModelValidator : AbstractValidator<TuChoiDuyetHoanTraVatTuViewModel>
    {
        public TuChoiDuyetHoanTraVatTuViewModelValidator(ILocalizationService localizationService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.LyDoHuy)
                .NotNull().WithMessage(localizationService.GetResource("DuyetHoanTra.LyDoHuy.Required"))
                .NotEmpty().WithMessage(localizationService.GetResource("DuyetHoanTra.LyDoHuy.Required"));
        }
    }
}
