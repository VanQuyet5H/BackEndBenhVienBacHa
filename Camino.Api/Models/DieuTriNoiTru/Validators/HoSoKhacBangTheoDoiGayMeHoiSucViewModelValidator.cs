using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HoSoKhacBangTheoDoiGayMeHoiSucViewModel>))]
    public class HoSoKhacBangTheoDoiGayMeHoiSucViewModelValidator : AbstractValidator<HoSoKhacBangTheoDoiGayMeHoiSucViewModel>
    {
        public HoSoKhacBangTheoDoiGayMeHoiSucViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NgayThucHien)
               .NotNull().WithMessage(localizationService.GetResource("HoSoKhac.NgayThucHien.Required"));
        }
    }
}