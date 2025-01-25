using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.DieuTriNoiTru;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThoiGianDieuTriSoSinhViewModel>))]

    public class PhieuThamKhamDieuTriSoSinhViewModelValidator : AbstractValidator<ThoiGianDieuTriSoSinhViewModel>
    {
        public PhieuThamKhamDieuTriSoSinhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.GioBatDau)
              .NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.ThoiGianDieuTriSoSinhViewModel.GioBatDau.Required"));

            RuleFor(x => x.GioKetThuc)
                  .NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.ThoiGianDieuTriSoSinhViewModel.GioKetThuc.Required"));
        }
    }
}
