using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DuyetHoanTraKSNK.DuyetHoanTraKSNKValidators
{
    [TransientDependency(ServiceType = typeof(IValidator<DuyetHoanTraKSNKViewModel>))]
    public class DuyetHoanTraKSNKViewModelValidator : AbstractValidator<DuyetHoanTraKSNKViewModel>
    {
        public DuyetHoanTraKSNKViewModelValidator(ILocalizationService localizationService)
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
