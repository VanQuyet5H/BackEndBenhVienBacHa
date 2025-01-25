using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThongTinHoSoPhieuChamSocSoSinhViewModel>))]

    public class ThongTinHoSoPhieuChamSocSoSinhViewModelValidator : AbstractValidator<ThongTinHoSoPhieuChamSocSoSinhViewModel>
    {
        public ThongTinHoSoPhieuChamSocSoSinhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Ngay)
                .NotEmpty().WithMessage(localizationService.GetResource("PhieuChamSoc.Ngay.Required"));

        }
    }
}
