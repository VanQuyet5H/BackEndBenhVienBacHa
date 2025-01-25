using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.NhatKyHeThong.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhatKyHeThongViewModel>))]
    public class NhatKyHeThongViewModelValidator: AbstractValidator<NhatKyHeThongViewModel>
    {
        public NhatKyHeThongViewModelValidator(ILocalizationService iLocalizationService)
        {
            RuleFor(x => x.NoiDung)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("NhatKyHeThong.NoiDung.Required"))
                .Length(0, 2000).WithMessage(iLocalizationService.GetResource("NhatKyHeThong.NoiDung.Length"));
            RuleFor(x => x.MaDoiTuong)
                .Length(0, 20).WithMessage(iLocalizationService.GetResource("NhatKyHeThong.MaDoiTuong.Length"));
        }
    }
}
