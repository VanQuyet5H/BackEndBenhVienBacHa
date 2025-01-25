using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.KhamDoan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TiepNhanDichVuChiDinhViewModelMultiselect>))]
    public class TiepNhanDichVuChiDinhViewModelMultiselectValidator : AbstractValidator<TiepNhanDichVuChiDinhViewModelMultiselect>
    {
        public TiepNhanDichVuChiDinhViewModelMultiselectValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.DichVus)
                .Must((model, input) => input != null && input.Count > 0)
                .WithMessage(localizationService.GetResource("KhamDoanTiepNhan.DichVus.Required"));
        }
    }
}
