using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.HoanTra.HoanTraValidators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuyetHoanTraVatTuViewModel>))]
    public class DuyetHoanTraVatTuViewModelValidator : AbstractValidator<DuyetHoanTraVatTuViewModel>
    {
        public DuyetHoanTraVatTuViewModelValidator(ILocalizationService localizationService)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(p => p.NguoiNhanId)
                .NotNull().WithMessage(localizationService.GetResource("DuyetHoanTraVatTu.NguoiNhan.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DuyetHoanTraVatTu.NguoiNhan.Required"));

            RuleFor(p => p.NguoiTraId)
                .NotNull().WithMessage(localizationService.GetResource("DuyetHoanTraVatTu.NguoiTra.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("DuyetHoanTraVatTu.NguoiTra.Required"));
        }
    }
}
