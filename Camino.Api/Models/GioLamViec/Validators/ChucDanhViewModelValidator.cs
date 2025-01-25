using Camino.Api.Models.GioLamViec;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.ChucDanh;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.ChucDanh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GioLamViecViewModel>))]
    public class GioLamViecViewModelValidator : AbstractValidator<GioLamViecViewModel>
    {
        public GioLamViecViewModelValidator(ILocalizationService iLocalizationService)
        {
            RuleFor(x => x.BatDau).NotEmpty().WithMessage(iLocalizationService.GetResource("Common.GioLamViec.BatDau.Required"));
            RuleFor(x => x.KetThuc).NotEmpty().WithMessage(iLocalizationService.GetResource("Common.GioLamViec.KetThuc.Required"));
        }
    }
}
