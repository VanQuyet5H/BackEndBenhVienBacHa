using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HoSoKhacGiayChuyenTuyenViewModel>))]
    public class HoSoKhacGiayChuyenTuyenViewModelValidator : AbstractValidator<HoSoKhacGiayChuyenTuyenViewModel>
    {
        public HoSoKhacGiayChuyenTuyenViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(p => p.NgayThucHien)
                .NotNull().WithMessage(localizationService.GetResource("HoSoKhac.NgayThucHien.Required"));
        }
    }
}
