using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.ChiSoXetNghiems;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.ChiSoXetNghiems.Validator
{
    [TransientDependency(ServiceType = typeof(IValidator<ChiSoXetNghiemViewModel>))]
    public class ChiSoXetNghiemViewModelValidator : AbstractValidator<ChiSoXetNghiemViewModel>
    {
        public ChiSoXetNghiemViewModelValidator(ILocalizationService iLocalizationService, IChiSoXetNghiemService chiSoXetNghiemService)
        {
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required"))
                .Must((model, s) => !chiSoXetNghiemService.CheckMaSoExits(model.Ma, model.Id))
                    .WithMessage(iLocalizationService.GetResource("Common.Ma.Exists"));

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required"))
                .Must((model, s) => !chiSoXetNghiemService.CheckTenExits(model.Ten, model.Id))
                    .WithMessage(iLocalizationService.GetResource("Common.Ten.Exists"));

            RuleFor(x => x.LoaiXetNghiem)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Loai.Required"));
        }
    }
}
