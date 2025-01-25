using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiemChungBenhNhanViewModel>))]
    public class TiemChungBenhNhanViewModelValidator : AbstractValidator<TiemChungBenhNhanViewModel>
    {
        public TiemChungBenhNhanViewModelValidator(ILocalizationService localizationService, IValidator<TiemChungBenhNhanTienSuBenhViewModel> tiemChungBenhNhanTienSuBenhViewModel, IValidator<TiemChungBenhNhanDiUngThuocViewModel> tiemChungBenhNhanDiUngThuocViewModel)
        {
            RuleForEach(p => p.BenhNhanTiemVacxinTienSuBenhs).SetValidator(tiemChungBenhNhanTienSuBenhViewModel);
            RuleForEach(p => p.BenhNhanTiemVacxinDiUngThuocs).SetValidator(tiemChungBenhNhanDiUngThuocViewModel);
        }
    }
}
