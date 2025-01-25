using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Services.NhomDichVuKyThuat;

namespace Camino.Api.Models.NhomDichVuKyThuat.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<NhomDichVuKyThuatViewModel>))]
    public class NhomDichVuKyThuatViewModelValidator  : AbstractValidator<NhomDichVuKyThuatViewModel>
    {
        public NhomDichVuKyThuatViewModelValidator(ILocalizationService iLocalizationService, INhomDichVuKyThuatService iNhomDichVuKyThuatService)
        {
            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ten.Required")).Length(0, 250).WithMessage(iLocalizationService
                    .GetResource("Common.Ten.Range"));
            RuleFor(x => x.Ma)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("Common.Ma.Required")).Length(0, 50).WithMessage(iLocalizationService
                    .GetResource("Common.Ma.Range"));

        }
    }
}
