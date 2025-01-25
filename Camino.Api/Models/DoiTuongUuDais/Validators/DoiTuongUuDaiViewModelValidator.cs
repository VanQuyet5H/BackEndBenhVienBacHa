using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.DoiTuongUuDais.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<DoiTuongUuDaiViewModel>))]
    public class DoiTuongUuDaiViewModelValidator : AbstractValidator<DoiTuongUuDaiViewModel>
    {
        public DoiTuongUuDaiViewModelValidator(ILocalizationService localizationService)
        {

            RuleFor(x => x.TiLeUuDai)
                .NotEmpty().WithMessage(localizationService.GetResource("DoiTuongUuDaiDichVuKyThuatAdd.TiLeUuDai.Required"));
            RuleFor(x => x.DoiTuongId)
               .NotEmpty().WithMessage(localizationService.GetResource("DoiTuongUuDaiDichVuKyThuatAdd.DoiTuongId.Required"));

        }
    }
}
