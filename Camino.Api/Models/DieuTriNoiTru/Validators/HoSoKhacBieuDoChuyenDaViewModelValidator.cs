using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HoSoKhacBieuDoChuyenDaViewModel>))]
    public class HoSoKhacBieuDoChuyenDaViewModelValidator : AbstractValidator<HoSoKhacBieuDoChuyenDaViewModel>
    {
        public HoSoKhacBieuDoChuyenDaViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NgayGhiBieuDo)
               .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.NgayGhiBieuDo.Required"));
        }
    }
}
