using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.XetNghiem.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<CapNhatGridItemChoDichVuDaCapCodeViewModel>))]
    public class CapNhatGridItemChoDichVuDaCapCodeViewModelValidator : AbstractValidator<CapNhatGridItemChoDichVuDaCapCodeViewModel>
    {
        public CapNhatGridItemChoDichVuDaCapCodeViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.NgayNhanMau)
                .NotEmpty().WithMessage(localizationService.GetResource("GridDaCapCode.NgayNhanMau.Required"));
        }
    }
}
