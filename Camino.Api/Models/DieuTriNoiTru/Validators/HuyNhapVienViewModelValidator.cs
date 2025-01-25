using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;

namespace Camino.Api.Models.DieuTriNoiTru.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<HuyNhapVienViewModel>))]
    public class HuyNhapVienViewModelValidator : AbstractValidator<HuyNhapVienViewModel>
    {
        public HuyNhapVienViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.LyDo)
                .NotEmpty().WithMessage(localizationService.GetResource("HuyNhapVien.LyDo.Required"));
        }
    }
}
