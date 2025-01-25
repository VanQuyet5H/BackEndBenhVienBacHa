using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.PhieuKhaiThacTienSuBenh.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<PhieuKhaiThacTienSuBenhViewModel>))]
    public class PhieuKhaiThacTienSuBenhViewModelValidator : AbstractValidator<PhieuKhaiThacTienSuBenhViewModel>
    {
        public PhieuKhaiThacTienSuBenhViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.BacSiKhaiThac)
                .NotEmpty().WithMessage(localizationService.GetResource("HoSoKhac.BacSiKhaiThac.Required"));
        }
    }
}
