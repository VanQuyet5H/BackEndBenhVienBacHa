using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.YeuCauKhamBenhKhamBoPhanKhac.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<YeuCauKhamBenhKhamBoPhanKhacViewModel>))]
    public class YeuCauKhamBenhKhamBoPhanKhacViewModelValidator : AbstractValidator<YeuCauKhamBenhKhamBoPhanKhacViewModel>
    {
        public YeuCauKhamBenhKhamBoPhanKhacViewModelValidator(ILocalizationService iLocalizationService)
        {

            RuleFor(x => x.Ten)
                .NotEmpty().WithMessage(iLocalizationService.GetResource("YeuCauKhamBenhKhamBoPhanKhac.Ten.Required"));
            RuleFor(x => x.NoiDUng)
               .NotEmpty().WithMessage(iLocalizationService.GetResource("YeuCauKhamBenhKhamBoPhanKhac.NoiDUng.Required"));
        }
    }
}
