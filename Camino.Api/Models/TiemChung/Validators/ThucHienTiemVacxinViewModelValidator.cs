using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.TiemChung.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<ThucHienTiemVacxinViewModel>))]
    public class ThucHienTiemVacxinViewModelValidator : AbstractValidator<ThucHienTiemVacxinViewModel>
    {
        public ThucHienTiemVacxinViewModelValidator(ILocalizationService localizationService, IValidator<ThucHienTiemTiemChungViewModel> tiemChunggValidator)
        {
            RuleFor(x => x.TiemChung).SetValidator(tiemChunggValidator);
        }
    }
}
