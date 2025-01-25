using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiemChungYeuCauDichVuKyThuatTiemChungViewModel>))]
    public class TiemChungYeuCauDichVuKyThuatTiemChungViewModelValidator : AbstractValidator<TiemChungYeuCauDichVuKyThuatTiemChungViewModel>
    {
        public TiemChungYeuCauDichVuKyThuatTiemChungViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(p => p.ViTriTiem)
                .NotNull().WithMessage(localizationService.GetResource("TiemChung.ViTriTiem.Required"));

            RuleFor(p => p.MuiSo)
                .NotNull().WithMessage(localizationService.GetResource("TiemChung.MuiSo.Required"))
                .NotEqual(0).WithMessage(localizationService.GetResource("TiemChung.MuiSo.Required"));
        }
    }
}
