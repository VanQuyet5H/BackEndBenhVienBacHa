using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiemChungYeuCauTiepNhanViewModel>))]
    public class TiemChungYeuCauTiepNhanValidator : AbstractValidator<TiemChungYeuCauTiepNhanViewModel>
    {
        public TiemChungYeuCauTiepNhanValidator(ILocalizationService localizationService, IValidator<TiemChungBenhNhanViewModel> tiemChungBenhNhanValidator)
        {
            RuleFor(p => p.BenhNhan).SetValidator(tiemChungBenhNhanValidator);
        }
    }
}