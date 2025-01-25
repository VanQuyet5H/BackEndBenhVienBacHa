using Camino.Core.DependencyInjection.Attributes;
using Camino.Services.Localization;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Camino.Api.Models.TrichBienBanHoiChan.Validators
{
    [TransientDependency(ServiceType = typeof(IValidator<TrichBienBanHoiChanViewModel>))]

    public class TrichBienBanHoiChanViewModelValidator : AbstractValidator<TrichBienBanHoiChanViewModel>
    {
        public TrichBienBanHoiChanViewModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ThoiGianHoiChan)
           .NotEmpty().WithMessage(localizationService.GetResource("DieuTriNoiTru.ThoiGianHoiChan.Required"));
        }
    }
}
