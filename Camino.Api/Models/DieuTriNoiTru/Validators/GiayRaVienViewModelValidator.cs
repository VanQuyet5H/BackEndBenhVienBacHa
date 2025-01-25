using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<GiayRaVienViewModel>))]

    public class GiayRaVienViewModelValidator : AbstractValidator<GiayRaVienViewModel>
    {
        public GiayRaVienViewModelValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.ThoiDiemThucHien)
            //.NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.ThoiDiemThucHien.Required"));
        }
    }
}
