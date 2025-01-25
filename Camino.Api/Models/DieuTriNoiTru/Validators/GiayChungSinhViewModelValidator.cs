using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GiayChungSinhViewModel>))]

    public class GiayChungSinhViewModelValidator : AbstractValidator<GiayChungSinhViewModel>
    {
        public GiayChungSinhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ThoiDiemThucHien)
            .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.ThoiDiemThucHien.Required"));
        }
    }
}
